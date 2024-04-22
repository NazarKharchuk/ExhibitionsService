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
    public class PainterService : IPainterService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PainterService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(PainterDTO entity)
        {
            ValidateEntity(entity);

            if (await uow.UserProfiles.GetByIdAsync(entity.ProfileId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ProfileId), "Профіль користувача з вказаним Id не існує");

            // Додати перевірку зв'язку 1 до 1 (або через авторизаційні дані взяти ІД)

            entity.PainterId = 0;
            await uow.Painters.CreateAsync(mapper.Map<Painter>(entity));
            await uow.SaveAsync();
        }

        public async Task<PainterDTO> UpdateAsync(PainterDTO entity)
        {
            ValidateEntity(entity);

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

        public async Task<Tuple<List<PainterInfoDTO>, int>> GetPagePainterInfoAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.Painters.GetAllAsync();
            int count = all.Count();
            if (pagination.PageNumber == null) pagination.PageNumber = 1;
            if (pagination.PageSize == null) { pagination.PageSize = 12; };
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 21);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            var allWithInfo = uow.Painters.GetAllPaintersWithInfo();
            allWithInfo = allWithInfo.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);

            var res = (await allWithInfo.ToListAsync()).Select(p => new PainterInfoDTO
                {
                    PainterId = p.PainterId,
                    Pseudonym = p.Pseudonym,
                    Description = p.Description,
                    ProfileId = p.ProfileId,
                    FirstName = p.UserProfile.FirstName,
                    LastName = p.UserProfile.LastName,
                    JoiningDate = p.UserProfile.JoiningDate,
                    LikesCount = p.Paintings.Any() ? p.Paintings.Sum(pg => pg.PaintingLikes.Count) : 0,
                    VictoriesCount = p.Paintings.Any() ? p.Paintings.SelectMany(pg => pg.ContestApplications).Count(ca => ca.IsWon) : 0,
                    RatingCount = p.Paintings.Any() ? p.Paintings.Sum(pg => pg.Ratings.Count) : 0,
                    AvgRating = p.Paintings.Any() && p.Paintings.SelectMany(painting => painting.Ratings).Any()
                        ? p.Paintings.SelectMany(painting => painting.Ratings).Average(r => r.RatingValue)
                        : 0
            }).ToList();
            return Tuple.Create(res, count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(PainterDTO entity)
        {
            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.Pseudonym.IsNullOrEmpty() || entity.Pseudonym.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Pseudonym));
        }

        private async Task<Painter?> CheckEntityPresence(int id)
        {
            Painter? existingEntity = await uow.Painters.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PainterDTO).Name, id);

            return existingEntity;
        }
    }
}
