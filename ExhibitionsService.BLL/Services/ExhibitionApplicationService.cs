using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.BLL.Services
{
    public class ExhibitionApplicationService : IExhibitionApplicationService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ExhibitionApplicationService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(ExhibitionApplicationDTO entity)
        {
            ValidateEntity(entity);

            if (await uow.Exhibitions.GetByIdAsync(entity.ExhibitionId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ExhibitionId), "Виставки з вказаним Id не існує");

            if (await uow.Paintings.GetByIdAsync(entity.PaintingId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            // додати перевірку кількості заявок від одного художника

            if ((await uow.ExhibitionApplications
                .FindAsync(x => x.ExhibitionId == entity.ExhibitionId && x.PaintingId == entity.PaintingId)).Count() >= 1)
                throw new ValidationException("Поточна картина вже була додана до цієї виставки.");

            entity.ApplicationId = 0;
            entity.IsConfirmed = false;
            await uow.ExhibitionApplications.CreateAsync(mapper.Map<ExhibitionApplication>(entity));
            await uow.SaveAsync();
        }

        public async Task ConfirmApplicationAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            existingEntity.IsConfirmed = true;

            await uow.ExhibitionApplications.UpdateAsync(existingEntity);
            await uow.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.ExhibitionApplications.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ExhibitionApplicationDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ExhibitionApplicationDTO>(existingEntity);
        }

        public async Task<List<ExhibitionApplicationDTO>> GetAllAsync()
        {
            return mapper.Map<List<ExhibitionApplicationDTO>>((await uow.ExhibitionApplications.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ExhibitionApplicationDTO entity)
        {

        }

        private async Task<ExhibitionApplication?> CheckEntityPresence(int id)
        {
            ExhibitionApplication? existingEntity = await uow.ExhibitionApplications.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ExhibitionApplicationDTO).Name, id);

            return existingEntity;
        }
    }
}
