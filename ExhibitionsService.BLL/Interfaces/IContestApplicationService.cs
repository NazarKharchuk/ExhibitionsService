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

        Task AddVoteAsync(int applicationId, int profileId);
        Task RemoveVoteAsync(int applicationId, int profileId);
        Task<int> VotesCountAsync(int applicationId);
    }
}
