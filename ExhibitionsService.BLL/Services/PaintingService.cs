using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.BLL.Services.Helpers;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Services
{
    public class PaintingService : IPaintingService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PaintingService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task<PaintingDTO> CreateAsync(PaintingDTO entity, IFormFile image)
        {
            ValidateEntity(entity);

            if (await uow.Painters.GetByIdAsync(entity.PainterId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterId), "Художника з вказаним Id не існує");

            ValidateImage(image);

            string imagePath = await uow.Images.SaveAsync(image);

            entity.PaintingId = 0;
            entity.ImagePath = imagePath;
            var savedEntity = await uow.Paintings.CreateAsync(mapper.Map<Painting>(entity));
            await uow.SaveAsync();
            return mapper.Map<PaintingDTO>(savedEntity);
        }

        public async Task<PaintingDTO> UpdateAsync(PaintingDTO entity, IFormFile? image)
        {
            var existingEntity = await CheckEntityPresence(entity.PaintingId);

            ValidateEntity(entity);

            if (existingEntity.IsSold is not null and true &&
                (existingEntity.IsSold != entity.IsSold || existingEntity.Price != entity.Price))
                throw new ValidationException("Не можна змінити ціну чи статус проданої картини");

            if (image != null)
            {
                ValidateImage(image);

                string newImagePath = await uow.Images.SaveAsync(image);
                uow.Images.Delete(existingEntity.ImagePath);

                existingEntity.ImagePath = newImagePath;
            }

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.CretionDate = entity.CretionDate;
            existingEntity.Width = entity.Width;
            existingEntity.Height = entity.Height;
            existingEntity.Location = entity.Location;
            existingEntity.Location = entity.Location;
            existingEntity.IsSold = entity.IsSold;
            existingEntity.Price = entity.Price;

            await uow.Paintings.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PaintingDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            uow.Images.Delete(existingEntity.ImagePath);

            await uow.Paintings.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PaintingDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<PaintingDTO>(existingEntity);
        }

        public async Task<List<PaintingDTO>> GetAllAsync()
        {
            return mapper.Map<List<PaintingDTO>>((await uow.Paintings.GetAllAsync()).ToList());
        }

        public async Task AddGenreAsync(int paintingId, int genreId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Genre? genre = await uow.Genres.GetByIdAsync(genreId);
            if (genre == null) throw new EntityNotFoundException(typeof(GenreDTO).Name, genreId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Genres.Any(pl => pl.GenreId == genreId));
            if (checkAvailability.Any())
                throw new ValidationException("Картина вже має цей жанр.");

            uow.Paintings.AddItem(painting.Genres, genre);
            await uow.SaveAsync();
        }

        public async Task RemoveGenreAsync(int paintingId, int genreId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Genre? genre = await uow.Genres.GetByIdAsync(genreId);
            if (genre == null) throw new EntityNotFoundException(typeof(GenreDTO).Name, genreId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Genres.Any(pl => pl.GenreId == genreId));
            if (!checkAvailability.Any())
                throw new ValidationException("Картина не має цього жанру.");

            uow.Paintings.RemoveItem(checkAvailability.FirstOrDefault().Genres, genre);
            await uow.SaveAsync();
        }

        public async Task AddStyleAsync(int paintingId, int styleId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Style? style = await uow.Styles.GetByIdAsync(styleId);
            if (style == null) throw new EntityNotFoundException(typeof(StyleDTO).Name, styleId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Styles.Any(pl => pl.StyleId == styleId));
            if (checkAvailability.Any())
                throw new ValidationException("Картина вже має цей стиль.");

            uow.Paintings.AddItem(painting.Styles, style);
            await uow.SaveAsync();
        }

        public async Task RemoveStyleAsync(int paintingId, int styleId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Style? style = await uow.Styles.GetByIdAsync(styleId);
            if (style == null) throw new EntityNotFoundException(typeof(StyleDTO).Name, styleId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Styles.Any(pl => pl.StyleId == styleId));
            if (!checkAvailability.Any())
                throw new ValidationException("Картина не має цього стилю.");

            uow.Paintings.RemoveItem(checkAvailability.FirstOrDefault().Styles, style);
            await uow.SaveAsync();
        }

        public async Task AddMaterialAsync(int paintingId, int materialId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Material? material = await uow.Materials.GetByIdAsync(materialId);
            if (material == null) throw new EntityNotFoundException(typeof(MaterialDTO).Name, materialId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Materials.Any(pl => pl.MaterialId == materialId));
            if (checkAvailability.Any())
                throw new ValidationException("Картина вже має цей матеріал.");

            uow.Paintings.AddItem(painting.Materials, material);
            await uow.SaveAsync();
        }

        public async Task RemoveMaterialAsync(int paintingId, int materialId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Material? material = await uow.Materials.GetByIdAsync(materialId);
            if (material == null) throw new EntityNotFoundException(typeof(MaterialDTO).Name, materialId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Materials.Any(pl => pl.MaterialId == materialId));
            if (!checkAvailability.Any())
                throw new ValidationException("Картина не має цього матеріалу.");

            uow.Paintings.RemoveItem(checkAvailability.FirstOrDefault().Materials, material);
            await uow.SaveAsync();
        }

        public async Task AddTagAsync(int paintingId, int tagId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Tags.Any(pl => pl.TagId == tagId));
            if (checkAvailability.Any())
                throw new ValidationException("Картина вже має цей тег.");

            uow.Paintings.AddItem(painting.Tags, tag);
            await uow.SaveAsync();
        }

        public async Task RemoveTagAsync(int paintingId, int tagId)
        {
            var painting = await CheckEntityPresence(paintingId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.Tags.Any(pl => pl.TagId == tagId));
            if (!checkAvailability.Any())
                throw new ValidationException("Картина не має цього тегу.");

            uow.Paintings.RemoveItem(checkAvailability.FirstOrDefault().Tags, tag);
            await uow.SaveAsync();
        }

        public async Task<PaintingInfoDTO> GetByIdWithInfoAsync(int id, ClaimsPrincipal claims)
        {
            var existingEntity = await CheckEntityPresence(id);

            var allWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var painting = allWithInfo.Where(p => p.PaintingId == id).FirstOrDefault();

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int? profileId = profileIdClaim != null ? int.Parse(profileIdClaim) : null;

            return mapper.Map<Painting, PaintingInfoDTO>(painting, opt =>
                    opt.AfterMap((src, dest) => dest.IsLiked =
                        profileId == null ? null : src.PaintingLikes.Any(pl => pl.ProfileId == profileId)));
        }

        public async Task<Tuple<List<PaintingInfoDTO>, int>> GetPagePaintingInfoAsync(
            PaintingsFiltrationPaginationRequestDTO filters,
            ClaimsPrincipal claims)
        {
            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            if (filters.PainterId != null)
            {
                Painter? existingPainter = await uow.Painters.GetByIdAsync((int)filters.PainterId);
                if (existingPainter == null) throw new EntityNotFoundException(typeof(PainterDTO).Name, (int)filters.PainterId);
                paintingsWithInfo = paintingsWithInfo.Where(p => p.PainterId == filters.PainterId);
            }
            if (filters.TagsIds != null)
            {
                foreach (var tagId in filters.TagsIds)
                {
                    if ((await uow.Tags.GetByIdAsync(tagId)) == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);
                    paintingsWithInfo = paintingsWithInfo.Where(p => p.Tags.Any(t => t.TagId == tagId));
                }
            }
            if (filters.GenresIds != null)
            {
                foreach (var genreId in filters.GenresIds)
                {
                    if ((await uow.Genres.GetByIdAsync(genreId)) == null) throw new EntityNotFoundException(typeof(GenreDTO).Name, genreId);
                    paintingsWithInfo = paintingsWithInfo.Where(p => p.Genres.Any(t => t.GenreId == genreId));
                }
            }
            if (filters.StylesIds != null)
            {
                foreach (var styleId in filters.StylesIds)
                {
                    if ((await uow.Styles.GetByIdAsync(styleId)) == null) throw new EntityNotFoundException(typeof(StyleDTO).Name, styleId);
                    paintingsWithInfo = paintingsWithInfo.Where(p => p.Styles.Any(t => t.StyleId == styleId));
                }
            }
            if (filters.MaterialsIds != null)
            {
                foreach (var materialId in filters.MaterialsIds)
                {
                    if ((await uow.Materials.GetByIdAsync(materialId)) == null) throw new EntityNotFoundException(typeof(MaterialDTO).Name, materialId);
                    paintingsWithInfo = paintingsWithInfo.Where(p => p.Materials.Any(t => t.MaterialId == materialId));
                }
            }
            if (filters.SortBy != null)
            {
                Func<Painting, object> sortSelector = filters.SortBy switch
                {
                    "PaintingId" => p => p.PaintingId,
                    "Name" => p => p.Name,
                    "CretionDate" => p => p.CretionDate,
                    _ => throw new ValidationException("Не привильне налаштування сортування")
                };
                if (filters.SortOrder != null)
                {
                    if (filters.SortOrder == "desc") paintingsWithInfo = paintingsWithInfo.OrderByDescending(sortSelector).AsQueryable();
                    else paintingsWithInfo = paintingsWithInfo.OrderBy(sortSelector).AsQueryable();
                }
            }

            return PaintingInfoResult(paintingsWithInfo, claims,
                new PaginationRequestDTO() { PageNumber = filters.PageNumber, PageSize = filters.PageSize });
        }

        private Tuple<List<PaintingInfoDTO>, int> PaintingInfoResult(
            IQueryable<Painting> paintingsWithInfo,
            ClaimsPrincipal claims,
            PaginationRequestDTO pagination)
        {
            int count = paintingsWithInfo.Count();
            pagination.PageNumber ??= 1;
            pagination.PageSize ??= 12;
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 21);
            if (pagination.PageNumber < 1 || pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            paintingsWithInfo = paintingsWithInfo.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int? profileId = profileIdClaim != null ? int.Parse(profileIdClaim) : null;

            var res = (paintingsWithInfo.ToList())
                .Select(p => mapper.Map<Painting, PaintingInfoDTO>(p, opt =>
                    opt.AfterMap((src, dest) => dest.IsLiked =
                        profileId == null ? null : src.PaintingLikes.Any(pl => pl.ProfileId == profileId)))
                ).ToList();
            return Tuple.Create(res, count);
        }

        public async Task AddLikeAsync(int paintingId, int profileId)
        {
            var painting = await CheckEntityPresence(paintingId);

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(UserProfileDTO).Name, profileId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.PaintingLikes.Any(pl => pl.ProfileId == profileId));
            if (checkAvailability.Any())
                throw new ValidationException("Користувач раніше вже вподобав цю картину.");

            var paintingLike = new PaintingLikeDTO()
            {
                PaintingId = paintingId,
                ProfileId = profileId,
                AddedTime = DateTime.Now
            };

            uow.Paintings.AddLike(painting, mapper.Map<PaintingLike>(paintingLike));
            await uow.SaveAsync();
        }

        public async Task RemoveLikeAsync(int paintingId, int profileId)
        {
            var painting = await CheckEntityPresence(paintingId);

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, profileId);

            var all = await uow.Paintings.GetAllPaintingsWithInfoAsync();
            var checkAvailability = all.Where(p =>
                p.PaintingId == paintingId &&
                p.PaintingLikes.Any(pl => pl.ProfileId == profileId));
            if (!checkAvailability.Any())
                throw new ValidationException("Користувач ще не вподобав цю картину.");

            uow.Paintings.RemoveLike(paintingId, profileId);
            await uow.SaveAsync();
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(PaintingDTO entity)
        {
            if (entity.Name.IsNullOrEmpty() || entity.Name.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Name));

            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.CretionDate > DateTime.Now)
                throw new ValidationException(entity.GetType().Name, nameof(entity.CretionDate));

            if (entity.Width <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Width));

            if (entity.Height <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Height));

            if (!entity.Location.IsNullOrEmpty() && entity.Location.Length > 100)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Location));

            if (entity.Price != null && entity.Price <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Price));

            if (entity.IsSold != null && entity.IsSold == false && entity.Price == null)
                throw new ValidationException("Якщо картина продається, то потрібно вказати її вартість");
        }

        private void ValidateImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ValidationException("PaintingDTO", "Image", "Файл зображення не був завантажений.");

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (extension.IsNullOrEmpty() || !allowedExtensions.Contains(extension))
                throw new ValidationException("PaintingDTO", "Image", "Дозволені формати зображення: jpg, jpeg, png, gif.");

            const int maxFileSize = 50 * 1024 * 1024; // 50 MB
            if (image.Length > maxFileSize)
                throw new ValidationException("PaintingDTO", "Image", "Розмір файлу перевищує допустимий ліміт (50 MB).");
        }

        private async Task<Painting?> CheckEntityPresence(int id)
        {
            Painting? existingEntity = await uow.Paintings.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, id);

            return existingEntity;
        }

        public async Task<StatisticsResponseDTO<StatisticsLikesValueDTO>?> GetLikesStatistics
            (int paintingId, DateTime periodStartDate, string periodSize)
        {
            await CheckEntityPresence(paintingId);

            IQueryable<PaintingLike> likes = uow.Paintings.GetPaintingLikes(paintingId);
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
            (int paintingId, DateTime periodStartDate, string periodSize)
        {
            await CheckEntityPresence(paintingId);

            IQueryable<PaintingRating> ratings = uow.Paintings.GetPaintingRatings(paintingId);
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
