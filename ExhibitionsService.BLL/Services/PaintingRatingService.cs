using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class PaintingRatingService : IPaintingRatingService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PaintingRatingService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(PaintingRatingDTO entity)
        {
            ValidateEntity(entity);

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            if (await uow.Paintings.GetByIdAsync(entity.PaintingId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            entity.RatingId = 0;
            entity.AddedDate = DateTime.Now;
            await uow.PaintingRatings.CreateAsync(mapper.Map<PaintingRating>(entity));
            await uow.SaveAsync();
        }

        public async Task<PaintingRatingDTO> UpdateAsync(PaintingRatingDTO entity)
        {
            ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.RatingId);

            existingEntity.RatingValue = entity.RatingValue;
            existingEntity.Comment = entity.Comment;
            existingEntity.AddedDate = DateTime.Now;

            await uow.PaintingRatings.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PaintingRatingDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.PaintingRatings.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PaintingRatingDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<PaintingRatingDTO>(existingEntity);
        }

        public async Task<List<PaintingRatingDTO>> GetAllAsync()
        {
            return mapper.Map<List<PaintingRatingDTO>>((await uow.PaintingRatings.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(PaintingRatingDTO entity)
        {
            if (entity.RatingValue < 0 || entity.RatingValue > 10)
                throw new ValidationException(entity.GetType().Name, nameof(entity.RatingValue));

            if (!entity.Comment.IsNullOrEmpty() && entity.Comment.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Comment));
        }

        private async Task<PaintingRating?> CheckEntityPresence(int id)
        {
            PaintingRating? existingEntity = await uow.PaintingRatings.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PaintingRatingDTO).Name, id);

            return existingEntity;
        }
    }
}
