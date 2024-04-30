using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
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
            await ValidateEntityAsync(entity);

            entity.TagId = 0;
            await uow.Tags.CreateAsync(mapper.Map<Tag>(entity));
            await uow.SaveAsync();
        }

        public async Task<TagDTO> UpdateAsync(TagDTO entity)
        {
            await ValidateEntityAsync(entity);

            var existingEntity = await CheckEntityPresence(entity.TagId);

            existingEntity.TagName = entity.TagName;

            await uow.Tags.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<TagDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Tags.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<TagDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<TagDTO>(existingEntity);
        }

        public async Task<List<TagDTO>> GetAllAsync()
        {
            return mapper.Map<List<TagDTO>>((await uow.Tags.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<TagDTO>, int>> GetPageAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.Tags.GetAllAsync();
            int count = all.Count();
            pagination.PageNumber ??= 1;
            pagination.PageSize ??= 10;;
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 20);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            all = all.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);
            return Tuple.Create(mapper.Map<List<TagDTO>>(all), count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private async Task ValidateEntityAsync(TagDTO entity)
        {
            if (entity.TagName.IsNullOrEmpty() || entity.TagName.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName));

            if ((await uow.Tags.FindAsync(t => t.TagName.Trim().ToLower().Equals(entity.TagName.Trim().ToLower()))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.TagName), "Тег повинен бути унікальним.");
        }

        private async Task<Tag?> CheckEntityPresence(int id)
        {
            Tag? existingEntity = await uow.Tags.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(TagDTO).Name, id);

            return existingEntity;
        }
    }
}
