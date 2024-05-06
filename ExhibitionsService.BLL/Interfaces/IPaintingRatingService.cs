using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingRatingService : IService<PaintingRatingDTO>
    {
        Task CreateAsync(PaintingRatingDTO entity, ClaimsPrincipal claims);
        Task<PaintingRatingDTO> UpdateAsync(PaintingRatingDTO entity, ClaimsPrincipal claims);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<PaintingRatingDTO?> GetByIdAsync(int id);
        Task<List<PaintingRatingDTO>> GetAllAsync();
        Task<PaintingRatingInfoDTO> GetByIdWithInfoAsync(int id);
        Task<Tuple<List<PaintingRatingInfoDTO>, int>> GetPagePaintingRatingInfoAsync(int paintingId, PaginationRequestDTO pagination);
        Task<PaintingRatingInfoDTO?> GetUserPaintingRating(int paintingId, ClaimsPrincipal claims);
    }
}
