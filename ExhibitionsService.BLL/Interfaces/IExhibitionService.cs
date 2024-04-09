using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IExhibitionService
    {
        Task CreateAsync(ExhibitionDTO entity);
        Task<ExhibitionDTO> UpdateAsync(ExhibitionDTO entity);
        Task DeleteAsync(int id);
        Task<ExhibitionDTO?> GetByIdAsync(int id);
        Task<List<ExhibitionDTO>> GetAllAsync();
    }
}
