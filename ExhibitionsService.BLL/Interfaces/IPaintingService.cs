using ExhibitionsService.BLL.DTO;
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
        
        Task AddLike(int paintingId, int profileId);
        Task RemoveLike(int paintingId, int profileId);
        Task<int> LikesCount(int paintingId);
    }
}
