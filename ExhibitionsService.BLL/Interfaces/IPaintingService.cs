using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingService : IService<PaintingDTO>
    {
        Task<PaintingDTO> CreateAsync(PaintingDTO entity, IFormFile image, ClaimsPrincipal claims);
        Task<PaintingDTO> UpdateAsync(PaintingDTO entity, IFormFile? image, ClaimsPrincipal claims);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<PaintingDTO?> GetByIdAsync(int id);
        Task<List<PaintingDTO>> GetAllAsync();

        Task AddGenreAsync(int paintingId, int genreId, ClaimsPrincipal claims);
        Task RemoveGenreAsync(int paintingId, int genreId, ClaimsPrincipal claims);
        Task AddStyleAsync(int paintingId, int styleId, ClaimsPrincipal claims);
        Task RemoveStyleAsync(int paintingId, int styleId, ClaimsPrincipal claims);
        Task AddMaterialAsync(int paintingId, int materialId, ClaimsPrincipal claims);
        Task RemoveMaterialAsync(int paintingId, int materialId, ClaimsPrincipal claims);
        Task AddTagAsync(int paintingId, int tagId, ClaimsPrincipal claims);
        Task RemoveTagAsync(int paintingId, int tagId, ClaimsPrincipal claims);

        Task<PaintingInfoDTO> GetByIdWithInfoAsync(int id, ClaimsPrincipal claims);
        Task<Tuple<List<PaintingInfoDTO>, int>> GetPagePaintingInfoAsync(
            PaintingsFiltrationPaginationRequestDTO filters,
            ClaimsPrincipal claims);


        Task AddLikeAsync(int paintingId, ClaimsPrincipal claims);
        Task RemoveLikeAsync(int paintingId, ClaimsPrincipal claims);

        Task<StatisticsResponseDTO<StatisticsLikesValueDTO>?> GetLikesStatistics
            (int paintingId, DateTime periodStartDate, string periodSize);
        Task<StatisticsResponseDTO<StatisticsRatingsValueDTO>?> GetRatingsStatistics
            (int paintingId, DateTime periodStartDate, string periodSize);
    }
}
