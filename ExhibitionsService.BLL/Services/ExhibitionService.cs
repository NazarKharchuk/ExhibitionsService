using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class ExhibitionService : IExhibitionService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ExhibitionService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(ExhibitionDTO entity)
        {
            ValidateEntity(entity);

            entity.ExhibitionId = 0;
            entity.AddedDate = DateTime.Now;
            await uow.Exhibitions.CreateAsync(mapper.Map<Exhibition>(entity));
            await uow.SaveAsync();
        }

        public async Task<ExhibitionDTO> UpdateAsync(ExhibitionDTO entity)
        {
            ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.ExhibitionId);

            if(existingEntity.PainterLimit > entity.PainterLimit)
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));
            }

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.NeedConfirmation = entity.NeedConfirmation;
            existingEntity.PainterLimit = entity.PainterLimit;

            await uow.Exhibitions.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<ExhibitionDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Exhibitions.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ExhibitionDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ExhibitionDTO>(existingEntity);
        }

        public async Task<List<ExhibitionDTO>> GetAllAsync()
        {
            return mapper.Map<List<ExhibitionDTO>>((await uow.Exhibitions.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ExhibitionDTO entity)
        {
            if (entity.Name.IsNullOrEmpty() || entity.Name.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Name));

            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.PainterLimit != null && entity.PainterLimit <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));
        }

        private async Task<Exhibition?> CheckEntityPresence(int id)
        {
            Exhibition? existingEntity = await uow.Exhibitions.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ExhibitionDTO).Name, id);

            return existingEntity;
        }
    }
}
