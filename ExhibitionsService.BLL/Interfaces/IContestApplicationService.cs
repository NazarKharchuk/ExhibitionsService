using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IContestApplicationService
    {
        Task CreateAsync(ContestApplicationDTO entity);
        Task ConfirmApplicationAsync(int id);
        Task ConfirmWinningAsync(int id);
        Task DeleteAsync(int id);
        Task<ContestApplicationDTO?> GetByIdAsync(int id);
        Task<List<ContestApplicationDTO>> GetAllAsync();
    }
}
