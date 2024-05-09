using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.BLL.Services.Helpers;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Enums;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class PainterService : IPainterService
    {
        private readonly IUnitOfWork uow;
        private readonly IUserProfileService profileService;
        private readonly IMapper mapper;

        public PainterService(IUnitOfWork _uow, IUserProfileService _profileService, IMapper _mapper)
        {
            uow = _uow;
            profileService = _profileService;
            mapper = _mapper;
        }

        public async Task CreateAsync(PainterDTO entity)
        {
            await ValidateEntity(entity);

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            if ((await uow.Painters.FindAsync(p => p.ProfileId.Equals(entity.ProfileId))).Any())
                throw new ValidationException("Цей користувач вже має обліковий запис художника.");

            entity.PainterId = 0;
            await uow.Painters.CreateAsync(mapper.Map<Painter>(entity));
            await uow.SaveAsync();
            
            var userEntities = await uow.UserProfiles.GetUserAndProfileByIdAsync(entity.ProfileId);
            if (userEntities.Item1 == null || userEntities.Item2 == null)
                throw new EntityNotFoundException(typeof(UserProfileDTO).Name, entity.ProfileId);
            await profileService.AddRole(userEntities.Item2.ProfileId, Role.Painter);
        }

        public async Task<PainterDTO> UpdateAsync(PainterDTO entity)
        {
            await ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.PainterId);

            existingEntity.Description = entity.Description;
            existingEntity.Pseudonym = entity.Pseudonym;

            await uow.Painters.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PainterDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await profileService.DeleteRole(existingEntity.ProfileId, Role.Painter);

            await uow.Painters.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PainterDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<PainterDTO>(existingEntity);
        }

        public async Task<PainterInfoDTO> GetByIdWithInfoAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            var allWithInfo = uow.Painters.GetAllPaintersWithInfo();
            var painter = allWithInfo.Where(p => p.PainterId == id).FirstOrDefault();

            return mapper.Map<PainterInfoDTO>(painter);
        }

        public async Task<List<PainterDTO>> GetAllAsync()
        {
            return mapper.Map<List<PainterDTO>>((await uow.Painters.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<PainterInfoDTO>, int>> GetPagePainterInfoAsync(PaintersFiltrationPaginationRequestDTO filters)
        {
            IQueryable<Painter> allWithInfo = uow.Painters.GetAllPaintersWithInfo();

            if (filters.SortBy != null)
            {
                Func<Painter, object> sortSelector = filters.SortBy switch
                {
                    "PainterId" => p => p.PainterId,
                    "Pseudonym" => p => p.Pseudonym,
                    _ => throw new ValidationException("Не привильне налаштування сортування")
                };
                if (filters.SortOrder != null)
                {
                    if (filters.SortOrder == "desc") allWithInfo = allWithInfo.OrderByDescending(sortSelector).AsQueryable();
                    else allWithInfo = allWithInfo.OrderBy(sortSelector).AsQueryable();
                }
            }

            int count = allWithInfo.Count();
            filters.PageNumber ??= 1;
            filters.PageSize ??= 12; ;
            filters.PageSize = Math.Min(filters.PageSize.Value, 21);
            if (filters.PageNumber < 1 ||
                filters.PageNumber < 1 ||
                (filters.PageNumber > (int)Math.Ceiling((double)count / filters.PageSize.Value) && count != 0) ||
                (count == 0 && filters.PageNumber != 1))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            allWithInfo = allWithInfo.Skip((int)((filters.PageNumber - 1) * filters.PageSize)).Take((int)filters.PageSize);

            var res = mapper.Map<List<PainterInfoDTO>>(allWithInfo.ToList());
            return Tuple.Create(res, count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private async Task ValidateEntity(PainterDTO entity)
        {
            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.Pseudonym.IsNullOrEmpty() || entity.Pseudonym.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Pseudonym));

            if ((await uow.Painters.FindAsync(p =>
                p.Pseudonym.Trim().ToLower() == entity.Pseudonym.Trim().ToLower() &&
                !p.PainterId.Equals(entity.PainterId)
            )).Any())
                throw new ValidationException(entity.GetType().Name, nameof(entity.Pseudonym), "Псевдонім повинен бути унікальним.");
        }

        private async Task<Painter?> CheckEntityPresence(int id)
        {
            Painter? existingEntity = await uow.Painters.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PainterDTO).Name, id);

            return existingEntity;
        }

        public async Task<StatisticsResponseDTO<StatisticsLikesValueDTO>?> GetLikesStatistics
            (int painterId, DateTime periodStartDate, string periodSize)
        {
            await CheckEntityPresence(painterId);

            IQueryable<PaintingLike> likes = uow.Painters.GetPainterLikes(painterId);
            if (!likes.Any()) return null;

            string dataFrequency = StatisticHelper.GetPeriodFrequency(periodSize);
            DateTime periodEndDate = StatisticHelper.CalculateIncreasedDate(periodStartDate, periodSize);
            DateTime veryFirstValue = likes.Min(l => l.AddedTime);
            DateTime veryLastValue = likes.Max(l => l.AddedTime);

            StatisticHelper.ValidateDatesForStatistic(periodStartDate, periodEndDate, veryFirstValue, dataFrequency, periodSize);

            var filteredLikes = likes.Where(l => l.AddedTime >= periodStartDate && l.AddedTime < periodEndDate);
            IQueryable<IGrouping<DateTime, PaintingLike>> groupedLikes = dataFrequency.ToLower() switch
            {
                "hour" => filteredLikes.GroupBy(l => new DateTime(l.AddedTime.Year, l.AddedTime.Month, l.AddedTime.Day, l.AddedTime.Hour, 0, 0)),
                "day" => filteredLikes.GroupBy(l => new DateTime(l.AddedTime.Year, l.AddedTime.Month, l.AddedTime.Day)),
                "month" => filteredLikes.GroupBy(l => new DateTime(l.AddedTime.Year, l.AddedTime.Month, 1)),
                "year" => filteredLikes.GroupBy(l => new DateTime(l.AddedTime.Year, 1, 1)),
                _ => throw new ValidationException("Неправильне значення частоти статистичних значень"),
            };
            var groupedLikesList = await groupedLikes.Select(g => new StatisticsLikesValueDTO
            {
                LikesCount = g.Count(),
                TimePeriodStart = g.Key,
                TimePeriodEnd = StatisticHelper.CalculateIncreasedDate(g.Key, dataFrequency)
            }).ToListAsync();

            var statistics = new StatisticsResponseDTO<StatisticsLikesValueDTO>
            {
                StatisticsValue = StatisticHelper.FillMissingIntervals<StatisticsLikesValueDTO>
                    (groupedLikesList, dataFrequency, periodStartDate, periodEndDate, (start, end) => new StatisticsLikesValueDTO
                    {
                        LikesCount = 0,
                        TimePeriodStart = start,
                        TimePeriodEnd = end
                    }),
                VeryFirstValue = veryFirstValue,
                VeryLastValue = veryLastValue
            };

            return statistics;
        }

        public async Task<StatisticsResponseDTO<StatisticsRatingsValueDTO>?> GetRatingsStatistics
            (int painterId, DateTime periodStartDate, string periodSize)
        {
            await CheckEntityPresence(painterId);

            IQueryable<PaintingRating> ratings = uow.Painters.GetPainterRatings(painterId);
            if (!ratings.Any()) return null;

            string dataFrequency = StatisticHelper.GetPeriodFrequency(periodSize);
            DateTime periodEndDate = StatisticHelper.CalculateIncreasedDate(periodStartDate, periodSize);
            DateTime veryFirstValue = ratings.Min(l => l.AddedDate);
            DateTime veryLastValue = ratings.Max(l => l.AddedDate);

            StatisticHelper.ValidateDatesForStatistic(periodStartDate, periodEndDate, veryFirstValue, dataFrequency, periodSize);

            var filteredRatings = ratings.Where(r => r.AddedDate >= periodStartDate && r.AddedDate < periodEndDate);
            IQueryable<IGrouping<DateTime, PaintingRating>> groupedRatings = dataFrequency.ToLower() switch
            {
                "hour" => filteredRatings.GroupBy(r => new DateTime(r.AddedDate.Year, r.AddedDate.Month, r.AddedDate.Day, r.AddedDate.Hour, 0, 0)),
                "day" => filteredRatings.GroupBy(r => new DateTime(r.AddedDate.Year, r.AddedDate.Month, r.AddedDate.Day)),
                "month" => filteredRatings.GroupBy(r => new DateTime(r.AddedDate.Year, r.AddedDate.Month, 1)),
                "year" => filteredRatings.GroupBy(r => new DateTime(r.AddedDate.Year, 1, 1)),
                _ => throw new ValidationException("Неправильне значення частоти статистичних значень"),
            };
            var groupedRatingsList = await groupedRatings.Select(g => new StatisticsRatingsValueDTO
            {
                RatingsCount = new RatingValueDTO(
                    g.Count(r => r.RatingValue >= 0 && r.RatingValue <= 1),
                    g.Count(r => r.RatingValue > 1 && r.RatingValue <= 2),
                    g.Count(r => r.RatingValue > 2 && r.RatingValue <= 3),
                    g.Count(r => r.RatingValue > 3 && r.RatingValue <= 4),
                    g.Count(r => r.RatingValue > 4 && r.RatingValue <= 5)
                ),
                TimePeriodStart = g.Key,
                TimePeriodEnd = StatisticHelper.CalculateIncreasedDate(g.Key, dataFrequency)
            }).ToListAsync();

            var statistics = new StatisticsResponseDTO<StatisticsRatingsValueDTO>
            {
                StatisticsValue = StatisticHelper.FillMissingIntervals<StatisticsRatingsValueDTO>
                    (groupedRatingsList, dataFrequency, periodStartDate, periodEndDate, (start, end) => new StatisticsRatingsValueDTO
                    {
                        RatingsCount = new RatingValueDTO(),
                        TimePeriodStart = start,
                        TimePeriodEnd = end
                    }),
                VeryFirstValue = veryFirstValue,
                VeryLastValue = veryLastValue
            };

            return statistics;
        }
    }
}
