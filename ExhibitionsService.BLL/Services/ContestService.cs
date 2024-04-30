using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            entity.AddedDate = DateTime.Now;
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

        public async Task<Tuple<List<ContestInfoDTO>, int>> GetPageContestInfoAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.Contests.GetAllAsync();
            int count = all.Count();
            pagination.PageNumber ??= 1;
            pagination.PageSize ??= 12;;
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 21);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            var allWithInfo = uow.Contests.GetAllContestsWithInfo();
            allWithInfo = allWithInfo.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);

            var res = mapper.Map<List<ContestInfoDTO>>(await allWithInfo.ToListAsync());
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
