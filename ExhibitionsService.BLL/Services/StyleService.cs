﻿using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class StyleService : IStyleService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public StyleService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(StyleDTO entity)
        {
            await ValidateEntityAsync(entity);

            entity.StyleId = 0;
            await uow.Styles.CreateAsync(mapper.Map<Style>(entity));
            await uow.SaveAsync();
        }

        public async Task<StyleDTO> UpdateAsync(StyleDTO entity)
        {
            await ValidateEntityAsync(entity);

            var existingEntity = await CheckEntityPresence(entity.StyleId);

            existingEntity.StyleName = entity.StyleName;

            await uow.Styles.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<StyleDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Styles.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<StyleDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<StyleDTO>(existingEntity);
        }

        public async Task<List<StyleDTO>> GetAllAsync()
        {
            return mapper.Map<List<StyleDTO>>((await uow.Styles.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<StyleDTO>, int>> GetPageAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.Styles.GetAllAsync();
            int count = all.Count();
            if (pagination.PageNumber == null) pagination.PageNumber = 1;
            if (pagination.PageSize == null) { pagination.PageSize = 10; };
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 20);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            all = all.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);
            return Tuple.Create(mapper.Map<List<StyleDTO>>(all), count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private async Task ValidateEntityAsync(StyleDTO entity)
        {
            if (entity.StyleName.IsNullOrEmpty() || entity.StyleName.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.StyleName));

            if ((await uow.Styles.FindAsync(t => t.StyleName.Trim().ToLower().Equals(entity.StyleName.Trim().ToLower()))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.StyleName), "Стиль повинен бути унікальним.");
        }

        private async Task<Style?> CheckEntityPresence(int id)
        {
            Style? existingEntity = await uow.Styles.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(StyleDTO).Name, id);

            return existingEntity;
        }
    }
}
