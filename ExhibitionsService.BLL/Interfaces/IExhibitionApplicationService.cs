using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IExhibitionApplicationService
    {
        Task CreateAsync(ExhibitionApplicationDTO entity);
        Task ConfirmApplicationAsync(int id);
        Task DeleteAsync(int id);
        Task<ExhibitionApplicationDTO?> GetByIdAsync(int id);
        Task<List<ExhibitionApplicationDTO>> GetAllAsync();
    }
}
