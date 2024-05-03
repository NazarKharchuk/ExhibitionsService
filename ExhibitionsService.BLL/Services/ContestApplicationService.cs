using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Services
{
    public class ContestApplicationService : IContestApplicationService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ContestApplicationService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(ContestApplicationDTO entity, ClaimsPrincipal claims)
        {
            ValidateEntity(entity);

            var contest = await uow.Contests.GetByIdAsync(entity.ContestId);
            if (contest == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ContestId), "Конкурсу з вказаним Id не існує");

            if (contest.StartDate <= DateTime.Now)
                throw new ValidationException("Подавати заявки на участь в конкурсі вже не можна");

            var painting = await uow.Paintings.GetByIdAsync(entity.PaintingId);
            if ( painting == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            if ((await uow.ContestApplications
                .FindAsync(ca => ca.ContestId == entity.ContestId && ca.PaintingId == entity.PaintingId))
                .Count() >= 1)
                throw new ValidationException("Заявка на участь цієї картини в цьому конкурсі раніше вже була подана");

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            string? painterIdClaim = claims.FindFirst("PainterId")?.Value;
            int? painterId = painterIdClaim != null ? int.Parse(painterIdClaim) : null;
            if (painterId == null) throw new ValidationException("Користувач не є художником");
            if (painterId != painting.PainterId)
                throw new InsufficientPermissionsException("Подати заявку на участь картини в конкурсі може тільки її автор");

            if (contest.PainterLimit != null)
            {
                paintingsWithInfo = paintingsWithInfo.Where(p => p.PainterId == painterId &&
                    p.ContestApplications.Any(ca => ca.ContestId == contest.ContestId));

                if (paintingsWithInfo.Count() >= contest.PainterLimit)
                    throw new ValidationException("Ліміт кількості заявок від одного художника вичерпано");
            }

            entity.ApplicationId = 0;
            entity.IsConfirmed = false;
            entity.IsWon = false;
            await uow.ContestApplications.CreateAsync(mapper.Map<ContestApplication>(entity));
            await uow.SaveAsync();
        }

        public async Task ConfirmApplicationAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            Contest? contest = await uow.Contests.GetByIdAsync(existingEntity.ContestId);
            if (contest == null) throw new EntityNotFoundException(typeof(ContestDTO).Name, existingEntity.ContestId);

            if (contest.EndDate < DateTime.Now)
                throw new ValidationException("Підтвердити заявку після закінчення конкурсу неможливо.");

            existingEntity.IsConfirmed = true;

            await uow.ContestApplications.UpdateAsync(existingEntity);
            await uow.SaveAsync();
        }

        public async Task ConfirmWinningAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            if ((await uow.ContestApplications
                    .FindAsync(x => x.ContestId == existingEntity.ContestId && x.IsWon == true))
                    .Count() >=
                (await uow.Contests
                    .FindAsync(x => x.ContestId == existingEntity.ContestId))
                    .FirstOrDefault()?.WinnersCount)
                throw new ValidationException("Конкурс вже має максимальну допустиму кількість переможців");

            existingEntity.IsWon = true;

            await uow.ContestApplications.UpdateAsync(existingEntity);
            await uow.SaveAsync();
        }

        public async Task DeleteAsync(int id, ClaimsPrincipal claims)
        {
            var existingEntity = await CheckEntityPresence(id);

            var roles = claims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin"))
            {
                string? painterIdClaim = claims.FindFirst("PainterId")?.Value;
                int painterId = painterIdClaim != null ?
                    int.Parse(painterIdClaim) :
                    throw new ValidationException("Користувач не є художником чи адміном");

                var painting = await uow.Paintings.GetByIdAsync(existingEntity.PaintingId) ??
                    throw new EntityNotFoundException(typeof(PaintingDTO).Name, existingEntity.PaintingId);
                if (painterId != painting.PainterId)
                    throw new InsufficientPermissionsException("Видалити заявку на участь картини в конкурсі може її автор чи адміністратор");
            }

            await uow.ContestApplications.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ContestApplicationDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ContestApplicationDTO>(existingEntity);
        }

        public async Task<List<ContestApplicationDTO>> GetAllAsync()
        {
            return mapper.Map<List<ContestApplicationDTO>>((await uow.ContestApplications.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<ContestApplicationDTO>, int>> GetPageAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.ContestApplications.GetAllAsync();
            int count = all.Count();
            pagination.PageNumber ??= 1;
            pagination.PageSize ??= 10;
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 20);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            all = all.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);
            return Tuple.Create(mapper.Map<List<ContestApplicationDTO>>(all), count);
        }

        public async Task AddVoteAsync(int applicationId, ClaimsPrincipal claims)
        {
            var application = await CheckEntityPresence(applicationId);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(UserProfileDTO).Name,profileId);

            Contest? contest = await uow.Contests.GetByIdAsync(application.ContestId);
            if (contest == null) throw new EntityNotFoundException(typeof(ContestDTO).Name, application.ContestId);

            if (contest.EndDate < DateTime.Now)
                throw new ValidationException("Віддати голос після закінчення конкурсу неможливо.");

            if (uow.ContestApplications.GetAllApplicationsWithinfo().Where(ca =>
                ca.ApplicationId == applicationId &&
                ca.Voters.Any(v => v.ProfileId == profileId)).Any())
                throw new ValidationException("Користувач раніше вже проголосував за цю картину.");

            if (contest.VotesLimit != null && uow.ContestApplications.GetAllApplicationsWithinfo().Where(ca =>
                ca.ContestId == application.ContestId &&
                ca.Voters.Any(v => v.ProfileId == profileId)).Count() >= (contest.VotesLimit ?? 0))
                throw new ValidationException("Користувач вже використав всі свої голоси.");

            uow.ContestApplications.AddVote(application, profile);
            await uow.SaveAsync();
        }

        public async Task RemoveVoteAsync(int applicationId, ClaimsPrincipal claims)
        {
            var application = await CheckEntityPresence(applicationId);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");

            Contest? contest = await uow.Contests.GetByIdAsync(application.ContestId);
            if (contest == null) throw new EntityNotFoundException(typeof(ContestDTO).Name, application.ContestId);

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, profileId);

            if (!uow.ContestApplications.GetAllApplicationsWithinfo().Where(ca =>
                ca.ApplicationId == applicationId &&
                ca.Voters.Any(v => v.ProfileId == profileId)).Any())
                throw new ValidationException("Користувач не проголосував за цю картину.");

            if (contest.EndDate < DateTime.Now)
                throw new ValidationException("Скасувати голос після закінчення конкурсу неможливо.");

            uow.ContestApplications.RemoveVote(applicationId, profileId);
            await uow.SaveAsync();
        }

        public async Task DetermineWinners()
        {
            Console.WriteLine("Початок визначення переможців конкурсів");

            var contests = uow.Contests.GetAllContestsWithInfo();

            var todayDate = DateTime.Now;
            var contestsToProcess = contests
                .Where(c => c.EndDate < todayDate && c.WinnersCount > c.Applications.Count(a => a.IsWon))
                .ToList();

            foreach (var contest in contestsToProcess)
            {
                IQueryable<ContestApplication> contestApps = uow.ContestApplications.GetAllApplicationsWithinfo().Where(ca => ca.Contest.ContestId == contest.ContestId);
                contestApps = contestApps.OrderByDescending(ca => ca.Voters.Count()).Take(contest.WinnersCount);

                foreach (var winner in contestApps.ToList())
                {
                    winner.IsWon = true;
                    await uow.ContestApplications.UpdateAsync(winner);
                }
            }
            await uow.SaveAsync();

            Console.WriteLine("Переможців конкурсів визначено");
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ContestApplicationDTO entity)
        {

        }

        private async Task<ContestApplication?> CheckEntityPresence(int id)
        {
            ContestApplication? existingEntity = await uow.ContestApplications.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ContestApplicationDTO).Name, id);

            return existingEntity;
        }
    }
}
