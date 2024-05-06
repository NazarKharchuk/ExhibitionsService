using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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

        public async Task CreateAsync(PaintingRatingDTO entity, ClaimsPrincipal claims)
        {
            ValidateEntity(entity);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");
            if (profileId != entity.ProfileId)
                throw new ValidationException("Користувач може оцінити картину тільки від свого імені");

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            if (await uow.Paintings.GetByIdAsync(entity.PaintingId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            if ((await uow.PaintingRatings
                .FindAsync(pr => pr.ProfileId == entity.ProfileId && pr.PaintingId == entity.PaintingId)).Any())
                    throw new ValidationException("Користувач вже оцінював цю картину раніше");

            entity.RatingId = 0;
            entity.AddedDate = DateTime.Now;
            await uow.PaintingRatings.CreateAsync(mapper.Map<PaintingRating>(entity));
            await uow.SaveAsync();
        }

        public async Task<PaintingRatingDTO> UpdateAsync(PaintingRatingDTO entity, ClaimsPrincipal claims)
        {
            ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.RatingId);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");
            if (profileId != existingEntity.ProfileId)
                throw new ValidationException("Оцінку картини може змінити тільки автор оцінки");

            existingEntity.RatingValue = entity.RatingValue;
            existingEntity.Comment = entity.Comment;
            existingEntity.AddedDate = DateTime.Now;

            await uow.PaintingRatings.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PaintingRatingDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id, ClaimsPrincipal claims)
        {
            var existingEntity = await CheckEntityPresence(id);

            var roles = claims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin"))
            {
                string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
                int profileId = profileIdClaim != null ?
                    int.Parse(profileIdClaim) :
                    throw new ValidationException("Користувач не авторизований");

                if (profileId != existingEntity.ProfileId)
                    throw new InsufficientPermissionsException("Видалити оцінку картини може її автор чи адміністратор");
            }

            await uow.PaintingRatings.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PaintingRatingDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<PaintingRatingDTO>(existingEntity);
        }

        public async Task<PaintingRatingInfoDTO> GetByIdWithInfoAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            var allWithInfo = uow.PaintingRatings.GetAllPaintingRatingsWithInfo();
            var rating = allWithInfo.Where(pr => pr.RatingId == id).FirstOrDefault();

            return mapper.Map<PaintingRating, PaintingRatingInfoDTO>(rating);
        }

        public async Task<List<PaintingRatingDTO>> GetAllAsync()
        {
            return mapper.Map<List<PaintingRatingDTO>>((await uow.PaintingRatings.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<PaintingRatingInfoDTO>, int>> GetPagePaintingRatingInfoAsync
            (int paintingId, PaginationRequestDTO pagination)
        {
            var allPaintingRatingsWithInfo = uow.PaintingRatings.GetAllPaintingRatingsWithInfo();
            allPaintingRatingsWithInfo = allPaintingRatingsWithInfo.Where(pr => pr.PaintingId == paintingId);

            int count = allPaintingRatingsWithInfo.Count();
            pagination.PageNumber ??= 1;
            pagination.PageSize ??= 12; ;
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 21);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            allPaintingRatingsWithInfo = allPaintingRatingsWithInfo.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);

            var res = mapper.Map<List<PaintingRatingInfoDTO>>((await allPaintingRatingsWithInfo.ToListAsync()));
            return Tuple.Create(res, count);
        }

        public async Task<PaintingRatingInfoDTO?> GetUserPaintingRating(int paintingId, ClaimsPrincipal claims)
        {
            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");

            if (await uow.Paintings.GetByIdAsync(paintingId) == null)
                throw new EntityNotFoundException(typeof(PaintingDTO).Name, paintingId);

            if (await uow.UserProfiles.GetByIdAsync(profileId) == null)
                throw new EntityNotFoundException(typeof(UserProfileDTO).Name, profileId);

            var allWithInfo = uow.PaintingRatings.GetAllPaintingRatingsWithInfo();
            allWithInfo = allWithInfo.Where(pr => pr.PaintingId == paintingId && pr.ProfileId == profileId);

            if (!allWithInfo.Any()) return null;
            else return mapper.Map<PaintingRatingInfoDTO>(await allWithInfo.FirstAsync());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(PaintingRatingDTO entity)
        {
            if (entity.RatingValue < 0 || entity.RatingValue > 5)
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
