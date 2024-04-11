using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using Microsoft.AspNetCore.Http;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingService : IService<PaintingDTO>
    {
        Task CreateAsync(PaintingDTO entity, IFormFile image);
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
        Task<List<PaintingInfoDTO>> GetAllWithInfoAsync();
        Task AddLike(int paintingId, int profileId);
        Task RemoveLike(int paintingId, int profileId);
        Task<int> LikesCount(int paintingId);
    }
}
