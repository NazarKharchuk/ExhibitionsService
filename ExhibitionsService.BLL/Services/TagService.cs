using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public TagService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(TagDTO entity)
        {
            if (entity.TagName.IsNullOrEmpty() || entity.TagName.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName));

            if ((await uow.Tags.FindAsync(t => t.TagName.Equals(entity.TagName))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName), "Тег повинен бути унікальним.");

            entity.TagId = 0;
            await uow.Tags.CreateAsync(mapper.Map<Tag>(entity));
            await uow.SaveAsync();
        }

        public async Task<TagDTO> UpdateAsync(TagDTO entity)
        {
            if (entity.TagName.IsNullOrEmpty() || entity.TagName.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName));

            if ((await uow.Tags.FindAsync(t => t.TagName.Equals(entity.TagName))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName), "Тег повинен бути унікальним.");

            var existingEntity = await uow.Tags.GetByIdAsync(entity.TagId);
            if (existingEntity == null) throw new EntityNotFoundException(entity.GetType().Name, entity.TagId);

            existingEntity.TagName = entity.TagName;

            await uow.Tags.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<TagDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await uow.Tags.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(TagDTO).Name, id);

            await uow.Tags.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<TagDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await uow.Tags.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(TagDTO).Name, id);

            return mapper.Map<TagDTO>(existingEntity);
        }

        public async Task<List<TagDTO>> GetAllAsync()
        {
            return mapper.Map<List<TagDTO>>((await uow.Tags.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }
}
