using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class PainterService : IPainterService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PainterService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(PainterDTO entity)
        {
            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.Pseudonym.IsNullOrEmpty() || entity.Pseudonym.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Pseudonym));

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            // Додати перевірку зв'язку 1 до 1 (або через авторизаційні дані взяти ІД)

            entity.PainterId = 0;
            await uow.Painters.CreateAsync(mapper.Map<Painter>(entity));
            await uow.SaveAsync();
        }

        public async Task<PainterDTO> UpdateAsync(PainterDTO entity)
        {
            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.Pseudonym.IsNullOrEmpty() || entity.Pseudonym.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Pseudonym));

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            var existingEntity = await uow.Painters.GetByIdAsync(entity.PainterId);
            if (existingEntity == null) throw new EntityNotFoundException(entity.GetType().Name, entity.PainterId);

            existingEntity.Description = entity.Description;
            existingEntity.Pseudonym = entity.Pseudonym;

            await uow.Painters.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PainterDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await uow.Painters.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PainterDTO).Name, id);

            await uow.Painters.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PainterDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await uow.Painters.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PainterDTO).Name, id);

            return mapper.Map<PainterDTO>(existingEntity);
        }

        public async Task<List<PainterDTO>> GetAllAsync()
        {
            return mapper.Map<List<PainterDTO>>((await uow.Painters.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }
}
