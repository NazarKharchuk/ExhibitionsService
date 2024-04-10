using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

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

        public async Task CreateAsync(ContestApplicationDTO entity)
        {
            ValidateEntity(entity);

            if (await uow.Contests.GetByIdAsync(entity.ContestId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ContestId), "Конкурсу з вказаним Id не існує");

            if (await uow.Paintings.GetByIdAsync(entity.PaintingId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            // додати перевірку кількості заявок від одного художника

            if ((await uow.ContestApplications
                .FindAsync(x => x.ContestId == entity.ContestId && x.PaintingId == entity.PaintingId))
                .Count() >= 1)
                throw new ValidationException("Поточна картина вже була додана до цього конкурсу.");

            entity.ApplicationId = 0;
            entity.IsConfirmed = false;
            entity.IsWon = false;
            await uow.ContestApplications.CreateAsync(mapper.Map<ContestApplication>(entity));
            await uow.SaveAsync();
        }

        public async Task ConfirmApplicationAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

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

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

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

        public async Task AddVoteAsync(int applicationId, int profileId)
        {
            var application = await CheckEntityPresence(applicationId);

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(UserProfileDTO).Name, profileId);

            Contest? contest = await uow.Contests.GetByIdAsync(application.ContestId);
            if (contest == null) throw new EntityNotFoundException(typeof(ContestDTO).Name, application.ContestId);

            if ((await uow.ContestApplications.FindApplicationsWithVoters(ca =>
                ca.ContestId == application.ContestId &&
                ca.Voters.Any(v => v.ProfileId == profileId))).ToArray().Length >= (contest.VotesLimit ?? 0))
                throw new ValidationException("Користувач вже використав всі свої голоси.");

            if ((await uow.ContestApplications.FindApplicationsWithVoters(ca =>
                ca.ApplicationId == applicationId &&
                ca.Voters.Any(v => v.ProfileId == profileId))).Any())
                throw new ValidationException("Користувач раніше вже проголосував за цю картину.");

            uow.ContestApplications.AddVote(application, profile);
            await uow.SaveAsync();
        }

        public async Task RemoveVoteAsync(int applicationId, int profileId)
        {
            var application = await CheckEntityPresence(applicationId);

            UserProfile? profile = await uow.UserProfiles.GetByIdAsync(profileId);
            if (profile == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, profileId);

            if (!(await uow.ContestApplications.FindApplicationsWithVoters(ca =>
                ca.ApplicationId == applicationId &&
                ca.Voters.Any(v => v.ProfileId == profileId))).Any())
                throw new ValidationException("Користувач ще не проголосував за цю картину.");

            uow.ContestApplications.RemoveVote(applicationId, profileId);
            await uow.SaveAsync();
        }

        public async Task<int> VotesCountAsync(int applicationId)
        {
            var existingEntity = await CheckEntityPresence(applicationId);

            return (await uow.ContestApplications.FindApplicationsWithVoters(ca => ca.ApplicationId == applicationId)).FirstOrDefault().Voters?.Count ?? 0;
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
