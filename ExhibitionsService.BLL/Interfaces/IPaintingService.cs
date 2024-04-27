using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingService : IService<PaintingDTO>
    {
        Task<PaintingDTO> CreateAsync(PaintingDTO entity, IFormFile image);
        Task<PaintingDTO> UpdateAsync(PaintingDTO entity, IFormFile? image);
        Task DeleteAsync(int id);
        Task<PaintingDTO?> GetByIdAsync(int id);
        Task<List<PaintingDTO>> GetAllAsync();

        Task AddGenreAsync(int paintingId, int genreId);
        Task RemoveGenreAsync(int paintingId, int genreId);
        Task AddStyleAsync(int paintingId, int styleId);
        Task RemoveStyleAsync(int paintingId, int styleId);
        Task AddMaterialAsync(int paintingId, int materialId);
        Task RemoveMaterialAsync(int paintingId, int materialId);
        Task AddTagAsync(int paintingId, int tagId);
        Task RemoveTagAsync(int paintingId, int tagId);

        Task<PaintingInfoDTO> GetByIdWithInfoAsync(int id, ClaimsPrincipal claims);
        Task<Tuple<List<PaintingInfoDTO>, int>> GetPagePaintingInfoAsync(PaginationRequestDTO pagination, ClaimsPrincipal claims);
        
        Task AddLikeAsync(int paintingId, int profileId);
        Task RemoveLikeAsync(int paintingId, int profileId);
    }
}
