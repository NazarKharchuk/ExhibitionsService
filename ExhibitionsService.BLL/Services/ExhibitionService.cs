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
    public class ExhibitionService : IExhibitionService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public ExhibitionService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task<ExhibitionDTO> CreateAsync(ExhibitionDTO entity)
        {
            ValidateEntity(entity);

            entity.ExhibitionId = 0;
            entity.AddedDate = DateTime.Now.Date;
            var savedEntity = await uow.Exhibitions.CreateAsync(mapper.Map<Exhibition>(entity));
            await uow.SaveAsync();
            return mapper.Map<ExhibitionDTO>(savedEntity);
        }

        public async Task<ExhibitionDTO> UpdateAsync(ExhibitionDTO entity)
        {
            ValidateEntity(entity);

            var existingEntity = await CheckEntityPresence(entity.ExhibitionId);

            if(existingEntity.PainterLimit > entity.PainterLimit)
            {
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));
            }

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.NeedConfirmation = entity.NeedConfirmation;
            existingEntity.PainterLimit = entity.PainterLimit;

            await uow.Exhibitions.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<ExhibitionDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            await uow.Exhibitions.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<ExhibitionDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<ExhibitionDTO>(existingEntity);
        }

        public async Task<List<ExhibitionDTO>> GetAllAsync()
        {
            return mapper.Map<List<ExhibitionDTO>>((await uow.Exhibitions.GetAllAsync()).ToList());
        }

        public async Task<ExhibitionInfoDTO> GetByIdWithInfoAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            var allWithInfo = uow.Exhibitions.GetAllExhibitionsWithInfo();
            var exhibition = allWithInfo.Where(e => e.ExhibitionId == id).FirstOrDefault();

            return mapper.Map<Exhibition, ExhibitionInfoDTO>(exhibition);
        }

        public async Task<Tuple<List<ExhibitionInfoDTO>, int>> GetPageExhibitionInfoAsync(ExhibitionFiltrationPaginationRequestDTO filters)
        {
            IQueryable<Exhibition> allWithInfo = uow.Exhibitions.GetAllExhibitionsWithInfo();

            if (filters.PaintingId != null)
            {
                Painting? existingPainting = await uow.Paintings.GetByIdAsync((int)filters.PaintingId);
                if (existingPainting == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, (int)filters.PaintingId);
                allWithInfo = allWithInfo.Where(e => e.Applications.Any(a => a.PaintingId == filters.PaintingId));
            }
            if (filters.TagsIds != null)
            {
                foreach (var tagId in filters.TagsIds)
                {
                    if ((await uow.Tags.GetByIdAsync(tagId)) == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);
                    allWithInfo = allWithInfo.Where(e => e.Tags.Any(t => t.TagId == tagId));
                }
            }
            if (filters.NeedConfirmation != null) allWithInfo = allWithInfo.Where(e => e.NeedConfirmation == filters.NeedConfirmation);
            if (filters.SortBy != null)
            {
                Func<Exhibition, object> sortSelector = filters.SortBy switch
                {
                    "Name" => e => e.Name,
                    "AddedDate" => e => e.AddedDate,
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

            var res = mapper.Map<List<ExhibitionInfoDTO>>(allWithInfo.ToList());
            return Tuple.Create(res, count);
        }

        public async Task AddTagAsync(int exhibitionId, int tagId)
        {
            var exhibition = await CheckEntityPresence(exhibitionId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = uow.Exhibitions.GetAllExhibitionsWithInfo();
            var checkAvailability = all.Where(e =>
                e.ExhibitionId == exhibitionId &&
                e.Tags.Any(t => t.TagId == tagId));
            if (checkAvailability.Any())
                throw new ValidationException("Виствка вже має цей тег.");

            uow.Exhibitions.AddItem(exhibition.Tags, tag);
            await uow.SaveAsync();
        }

        public async Task RemoveTagAsync(int exhibitionId, int tagId)
        {
            var exhibition = await CheckEntityPresence(exhibitionId);

            Tag? tag = await uow.Tags.GetByIdAsync(tagId);
            if (tag == null) throw new EntityNotFoundException(typeof(TagDTO).Name, tagId);

            var all = uow.Exhibitions.GetAllExhibitionsWithInfo();
            var checkAvailability = all.Where(e =>
                e.ExhibitionId == exhibitionId &&
                e.Tags.Any(t => t.TagId == tagId));
            if (!checkAvailability.Any())
                throw new ValidationException("Виставка не має цього тегу.");

            uow.Exhibitions.RemoveItem(checkAvailability.FirstOrDefault().Tags, tag);
            await uow.SaveAsync();
        }

        public async Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPageExhibitionApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId)
        {
            var exhibition = await CheckEntityPresence(exhibitionId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            if (exhibition.NeedConfirmation) paintingsWithInfo = paintingsWithInfo.Where(p => p.ExhibitionApplications.
                Any(ea => ea.ExhibitionId == exhibitionId && ea.IsConfirmed));
            else paintingsWithInfo = paintingsWithInfo.Where(p => p.ExhibitionApplications.
                Any(ea => ea.ExhibitionId == exhibitionId));

            return await ExhibitionApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, exhibitionId);
        }

        public async Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPageExhibitionNotConfirmedApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId)
        {
            var exhibition = await CheckEntityPresence(exhibitionId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            paintingsWithInfo = paintingsWithInfo.Where(p => p.ExhibitionApplications.
                Any(ea => ea.ExhibitionId == exhibitionId && ea.IsConfirmed == false));

            return await ExhibitionApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, exhibitionId);
        }

        public async Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPainterExhibitionSubmissionsAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId)
        {
            var exhibition = await CheckEntityPresence(exhibitionId);

            var paintingsWithInfo = await uow.Paintings.GetAllPaintingsWithInfoAsync();

            string? painterIdClaim = claims.FindFirst("PainterId")?.Value;
            int? painterId = painterIdClaim != null ? int.Parse(painterIdClaim) : null;
            if (painterId == null) throw new ValidationException("Користувач не є художником");

            paintingsWithInfo = paintingsWithInfo.Where(p => p.PainterId == painterId &&
                p.ExhibitionApplications.Any(ea => ea.ExhibitionId == exhibitionId));

            return await ExhibitionApplicationInfoResultAsync(paintingsWithInfo, claims, pagination, exhibitionId);
        }

        private async Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> ExhibitionApplicationInfoResultAsync
            (IQueryable<Painting> paintingsWithInfo, ClaimsPrincipal claims, PaginationRequestDTO pagination, int exhibitionId)
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
                    var application = p.ExhibitionApplications.First(ea => ea.PaintingId == p.PaintingId && ea.ExhibitionId == exhibitionId);

                    var paintingInfoDTO = mapper.Map<Painting, PaintingInfoDTO>(p, opt =>
                        opt.AfterMap((src, dest) =>
                            dest.IsLiked = profileId == null
                                ? null
                                : src.PaintingLikes.Any(pl => pl.ProfileId == profileId)));

                    return new ExhibitionApplicationInfoDTO
                    {
                        ApplicationId = application.ApplicationId,
                        IsConfirmed = application.IsConfirmed,
                        ExhibitionId = exhibitionId,
                        PaintingId = p.PaintingId,
                        Painting = paintingInfoDTO,
                    };
                }).ToList();
            return Tuple.Create(res, count);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(ExhibitionDTO entity)
        {
            if (entity.Name.IsNullOrEmpty() || entity.Name.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Name));

            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.PainterLimit != null && entity.PainterLimit <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterLimit));
        }

        private async Task<Exhibition?> CheckEntityPresence(int id)
        {
            Exhibition? existingEntity = await uow.Exhibitions.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(ExhibitionDTO).Name, id);

            return existingEntity;
        }
    }
}
