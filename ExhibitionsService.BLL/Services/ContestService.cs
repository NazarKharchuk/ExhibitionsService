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
    public class ContestService : IContestService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ContestService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task<ContestDTO> CreateAsync(ContestDTO entity)
        {
            ValidateEntity(entity);

            if (entity.StartDate < DateTime.Now)
                throw new ValidationException(entity.GetType().Name, nameof(entity.StartDate));

            entity.ContestId = 0;
            entity.AddedDate = DateTime.Now.Date;
            var savedEntity = await uow.Contests.CreateAsync(mapper.Map<Contest>(entity));
            await uow.SaveAsync();
            return mapper.Map<ContestDTO>(savedEntity);
        }

        public async Task<ContestDTO> UpdateAsync(ContestDTO entity)
        {
            ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.ContestId);

            if (entity.StartDate != existingEntity.StartDate &&
                (existingEntity.StartDate < DateTime.Now || entity.StartDate < DateTime.Now))
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.StartDate));
            }

            if (entity.EndDate != existingEntity.EndDate && existingEntity.EndDate < DateTime.Now)
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.EndDate));
            }

            if (entity.NeedConfirmation != existingEntity.NeedConfirmation && entity.StartDate < DateTime.Now)
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.NeedConfirmation));
            }

            if (entity.PainterLimit != existingEntity.PainterLimit &&
                (existingEntity.PainterLimit > entity.PainterLimit || entity.StartDate < DateTime.Now))
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));
            }

            if (entity.VotesLimit != existingEntity.VotesLimit &&
                (existingEntity.VotesLimit > entity.VotesLimit || entity.EndDate < DateTime.Now))
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.VotesLimit));
            }

            if (entity.WinnersCount != existingEntity.WinnersCount &&
                (existingEntity.WinnersCount > entity.WinnersCount || entity.EndDate < DateTime.Now))
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.WinnersCount));
            }

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.StartDate = entity.StartDate;
            existingEntity.EndDate = entity.EndDate;
            existingEntity.NeedConfirmation = entity.NeedConfirmation;
            existingEntity.PainterLimit = entity.PainterLimit;
            existingEntity.VotesLimit = entity.VotesLimit;
            existingEntity.WinnersCount = entity.WinnersCount;

            await uow.Contests.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<ContestDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Contests.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ContestDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ContestDTO>(existingEntity);
        }

        public async Task<ContestInfoDTO> GetByIdWithInfoAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            var allWithInfo = uow.Contests.GetAllContestsWithInfo();
            var contest = allWithInfo.Where(c => c.ContestId == id).FirstOrDefault();

            return mapper.Map<Contest, ContestInfoDTO>(contest);
        }

        public async Task<List<ContestDTO>> GetAllAsync()
        {
            return mapper.Map<List<ContestDTO>>((await uow.Contests.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<ContestInfoDTO>, int>> GetPageContestInfoAsync(ContestsFiltrationPaginationRequestDTO filters)
        {
            IQueryable<Contest> allWithInfo = uow.Contests.GetAllContestsWithInfo();

            if (filters.PaintingId != null)
            {
                Painting? existingPainting = await uow.Paintings.GetByIdAsync((int)filters.PaintingId);
                if (existingPainting == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, (int)filters.PaintingId);
                allWithInfo = allWithInfo.Where(c => c.Applications.Any(a => a.PaintingId == filters.PaintingId));
            }
            if (filters.TagsIds != null)
            {
                foreach(var tagId in filters.TagsIds)
                {
                    if ((await uow.Tags.GetByIdAsync(tagId)) == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);
                    allWithInfo = allWithInfo.Where(c => c.Tags.Any(t => t.TagId == tagId));
                }
            }
            if (filters.NeedConfirmation != null) allWithInfo = allWithInfo.Where(c => c.NeedConfirmation == filters.NeedConfirmation);
            if (filters.Status != null)
            {
                DateTime now = DateTime.Now;
                if (filters.Status == "ApplicationOpen") allWithInfo = allWithInfo.Where(c => c.StartDate > now); 
                else if (filters.Status == "Voting") allWithInfo = allWithInfo.Where(c => c.StartDate <= now && c.EndDate > now);
                else if (filters.Status == "Closed") allWithInfo = allWithInfo.Where(c => c.EndDate <= now);
            }
            if (filters.SortBy != null)
            {
                Func<Contest, object> sortSelector = filters.SortBy switch
                {
                    "Name" => c => c.Name,
                    "AddedDate" => c => c.AddedDate,
                    "StartDate" => c => c.StartDate,
                    "EndDate" => c => c.EndDate,
                    _ => throw new ValidationException("Не привильне налаштування сортування")
                };
                if(filters.SortOrder != null)
                {
                    if(filters.SortOrder == "desc") allWithInfo = allWithInfo.OrderByDescending(sortSelector).AsQueryable();
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

            var res = mapper.Map<List<ContestInfoDTO>>(allWithInfo.ToList());
            return Tuple.Create(res, count);
        }

        public async Task AddTagAsync(int contestId, int tagId)
        {
            var contest = await CheckEntityPresence(contestId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = uow.Contests.GetAllContestsWithInfo();
            var checkAvailability = all.Where(p =>
                p.ContestId == contestId &&
                p.Tags.Any(pl => pl.TagId == tagId));
            if (checkAvailability.Any())
                throw new ValidationException("Конкурс вже має цей тег.");

            uow.Contests.AddItem(contest.Tags, tag);
            await uow.SaveAsync();
        }

        public async Task RemoveTagAsync(int contestId, int tagId)
        {
            var contest = await CheckEntityPresence(contestId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = uow.Contests.GetAllContestsWithInfo();
            var checkAvailability = all.Where(p =>
                p.ContestId == contestId &&
                p.Tags.Any(pl => pl.TagId == tagId));
            if (!checkAvailability.Any())
                throw new ValidationException("Конкурс не має цього тегу.");

            uow.Contests.RemoveItem(checkAvailability.FirstOrDefault().Tags, tag);
            await uow.SaveAsync();
        }

        public async Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPageContestApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId)
        {
            var contest = await CheckEntityPresence(contestId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            if (contest.NeedConfirmation) paintingsWithInfo = paintingsWithInfo.Where(p => p.ContestApplications.
                Any(ca => ca.ContestId == contestId && ca.IsConfirmed));
            else paintingsWithInfo = paintingsWithInfo.Where(p => p.ContestApplications.
                Any(ca => ca.ContestId == contestId));

            if (contest.EndDate < DateTime.Now) paintingsWithInfo = paintingsWithInfo.
                    OrderByDescending(p => p.ContestApplications.First(ca => ca.ContestId == contestId).Voters.Count());

            return await ContestApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, contestId);
        }

        public async Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPageContestNotConfirmedApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId)
        {
            var contest = await CheckEntityPresence(contestId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            paintingsWithInfo = paintingsWithInfo.Where(p => p.ContestApplications.
                Any(ca => ca.ContestId == contestId && ca.IsConfirmed == false));

            return await ContestApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, contestId);
        }

        public async Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPainterContestSubmissionsAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId)
        {
            var contest = await CheckEntityPresence(contestId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            string? painterIdClaim = claims.FindFirst("PainterId")?.Value;
            int? painterId = painterIdClaim != null ? int.Parse(painterIdClaim) : null;
            if (painterId == null) throw new ValidationException("Користувач не є художником");

            paintingsWithInfo = paintingsWithInfo.Where(p => p.PainterId == painterId &&
                p.ContestApplications.Any(ca => ca.ContestId == contestId));

            return await ContestApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, contestId);
        }

        public async Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetUserContestVotesAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId)
        {
            var contest = await CheckEntityPresence(contestId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int? profileId = profileIdClaim != null ? int.Parse(profileIdClaim) : null;
            if (profileId == null) throw new ValidationException("Користувач не авторизований");

            paintingsWithInfo = paintingsWithInfo.Where(p => p.ContestApplications.Any(
                ca => ca.ContestId == contestId &&
                ca.Voters.Any(p => p.ProfileId == profileId)));

            return await ContestApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, contestId);
        }

        private async Task<Tuple<List<ContestApplicationInfoDTO>, int>> ContestApplicationInfoResultAsync
            (IQueryable<Painting> paintingsWithInfo, ClaimsPrincipal claims, PaginationRequestDTO pagination, int contestId)
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

            var res = (await paintingsWithInfo.ToListAsync())
                .Select(p =>
                {
                    var application = p.ContestApplications.First(ca => ca.PaintingId == p.PaintingId && ca.ContestId == contestId);

                    var paintingInfoDTO = mapper.Map<Painting, PaintingInfoDTO>(p, opt =>
                        opt.AfterMap((src, dest) =>
                            dest.IsLiked = profileId == null
                                ? null
                                : src.PaintingLikes.Any(pl => pl.ProfileId == profileId)));

                    return new ContestApplicationInfoDTO
                    {
                        ApplicationId = application.ApplicationId,
                        IsConfirmed = application.IsConfirmed,
                        IsWon = application.IsWon,
                        ContestId = contestId,
                        PaintingId = p.PaintingId,
                        Painting = paintingInfoDTO,
                        VotesCount = application.Voters.Count(),
                    };
                }).ToList();
            return Tuple.Create(res, count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ContestDTO entity)
        {
            if (entity.Name.IsNullOrEmpty() || entity.Name.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Name));

            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.EndDate < entity.StartDate)
                throw new ValidationException(entity.GetType().Name, nameof(entity.EndDate));

            if (entity.WinnersCount <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.WinnersCount));

            if (entity.PainterLimit != null && entity.PainterLimit <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));

            if (entity.VotesLimit != null && entity.VotesLimit <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.VotesLimit));
        }

        private async Task<Contest?> CheckEntityPresence(int id)
        {
            Contest? existingEntity = await uow.Contests.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ContestDTO).Name, id);

            return existingEntity;
        }
    }
}
