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
    public class ExhibitionApplicationService : IExhibitionApplicationService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ExhibitionApplicationService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(ExhibitionApplicationDTO entity, ClaimsPrincipal claims)
        {
            ValidateEntity(entity);

            var exhibition = await uow.Exhibitions.GetByIdAsync(entity.ExhibitionId);
            if (exhibition == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.ExhibitionId), "Виставки з вказаним Id не існує");

            var painting = await uow.Paintings.GetByIdAsync(entity.PaintingId);
            if (painting == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PaintingId), "Картини з вказаним Id не існує");

            if ((await uow.ExhibitionApplications
                .FindAsync(ea => ea.ExhibitionId == entity.ExhibitionId && ea.PaintingId == entity.PaintingId)).Any())
                throw new ValidationException("Поточна картина вже була додана до цієї виставки.");

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            string? painterIdClaim = claims.FindFirst("PainterId")?.Value;
            int? painterId = painterIdClaim != null ? int.Parse(painterIdClaim) : null;
            if (painterId == null) throw new ValidationException("Користувач не є художником");
            if (painterId != painting.PainterId)
                throw new InsufficientPermissionsException("Подати заявку на участь картини в конкурсі може тільки її автор");

            if (exhibition.PainterLimit != null)
            {
                paintingsWithInfo = paintingsWithInfo.Where(p => p.PainterId == painterId &&
                    p.ExhibitionApplications.Any(ea => ea.ExhibitionId == exhibition.ExhibitionId));

                if (paintingsWithInfo.Count() >= exhibition.PainterLimit)
                    throw new ValidationException("Ліміт кількості заявок від одного художника вичерпано");
            }

            entity.ApplicationId = 0;
            entity.IsConfirmed = false;
            await uow.ExhibitionApplications.CreateAsync(mapper.Map<ExhibitionApplication>(entity));
            await uow.SaveAsync();
        }

        public async Task ConfirmApplicationAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            existingEntity.IsConfirmed = true;

            await uow.ExhibitionApplications.UpdateAsync(existingEntity);
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

            await uow.ExhibitionApplications.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ExhibitionApplicationDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ExhibitionApplicationDTO>(existingEntity);
        }

        public async Task<List<ExhibitionApplicationDTO>> GetAllAsync()
        {
            return mapper.Map<List<ExhibitionApplicationDTO>>((await uow.ExhibitionApplications.GetAllAsync()).ToList());
        }

        public async Task<Tuple<List<ExhibitionApplicationDTO>, int>> GetPageAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.ExhibitionApplications.GetAllAsync();
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
            return Tuple.Create(mapper.Map<List<ExhibitionApplicationDTO>>(all), count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ExhibitionApplicationDTO entity)
        {

        }

        private async Task<ExhibitionApplication?> CheckEntityPresence(int id)
        {
            ExhibitionApplication? existingEntity = await uow.ExhibitionApplications.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ExhibitionApplicationDTO).Name, id);

            return existingEntity;
        }
    }
}
