using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingRatingService : IService<PaintingRatingDTO>
    {
        Task CreateAsync(PaintingRatingDTO entity);
        Task<PaintingRatingDTO> UpdateAsync(PaintingRatingDTO entity);
        Task DeleteAsync(int id);
        Task<PaintingRatingDTO?> GetByIdAsync(int id);
        Task<List<PaintingRatingDTO>> GetAllAsync();
    }
}
