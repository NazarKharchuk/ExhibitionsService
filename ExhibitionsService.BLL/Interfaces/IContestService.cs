using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IContestService
    {
        Task CreateAsync(ContestDTO entity);
        Task<ContestDTO> UpdateAsync(ContestDTO entity);
        Task DeleteAsync(int id);
        Task<ContestDTO?> GetByIdAsync(int id);
        Task<List<ContestDTO>> GetAllAsync();
    }
}
