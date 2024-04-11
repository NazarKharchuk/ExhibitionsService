﻿using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public GenreService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(GenreDTO entity)
        {
            await ValidateEntityAsync(entity);

            entity.GenreId = 0;
            await uow.Genres.CreateAsync(mapper.Map<Genre>(entity));
            await uow.SaveAsync();
        }

        public async Task<GenreDTO> UpdateAsync(GenreDTO entity)
        {
            await ValidateEntityAsync(entity);

            var existingEntity = await CheckEntityPresence(entity.GenreId);

            existingEntity.GenreName = entity.GenreName;

            await uow.Genres.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<GenreDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Genres.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<GenreDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<GenreDTO>(existingEntity);
        }

        public async Task<List<GenreDTO>> GetAllAsync()
        {
            return mapper.Map<List<GenreDTO>>((await uow.Genres.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private async Task ValidateEntityAsync(GenreDTO entity)
        {
            if (entity.GenreName.IsNullOrEmpty() || entity.GenreName.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.GenreName));

            if ((await uow.Genres.FindAsync(t => t.GenreName.Trim().ToLower().Equals(entity.GenreName.Trim().ToLower()))).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.GenreName), "Жанр повинен бути унікальним.");
        }

        private async Task<Genre?> CheckEntityPresence(int id)
        {
            Genre? existingEntity = await uow.Genres.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(GenreDTO).Name, id);

            return existingEntity;
        }
    }
}
