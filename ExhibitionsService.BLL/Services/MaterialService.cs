using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public MaterialService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(MaterialDTO entity)
        {
            await ValidateEntityAsync(entity);

            entity.MaterialId = 0;
            await uow.Materials.CreateAsync(mapper.Map<Material>(entity));
            await uow.SaveAsync();
        }

        public async Task<MaterialDTO> UpdateAsync(MaterialDTO entity)
        {
            await ValidateEntityAsync(entity);

            var existingEntity = await CheckEntityPresence(entity.MaterialId);

            existingEntity.MaterialName = entity.MaterialName;

            await uow.Materials.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<MaterialDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Materials.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<MaterialDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<MaterialDTO>(existingEntity);
        }

        public async Task<List<MaterialDTO>> GetAllAsync()
        {
            return mapper.Map<List<MaterialDTO>>((await uow.Materials.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private async Task ValidateEntityAsync(MaterialDTO entity)
        {
            if (entity.MaterialName.IsNullOrEmpty() || entity.MaterialName.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.MaterialName));

            if ((await uow.Materials.FindAsync(t => t.MaterialName.Trim().ToLower().Equals(entity.MaterialName.Trim().ToLower()))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.MaterialName), "Матеріал повинен бути унікальним.");
        }

        private async Task<Material?> CheckEntityPresence(int id)
        {
            Material? existingEntity = await uow.Materials.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(MaterialDTO).Name, id);

            return existingEntity;
        }
    }
}
