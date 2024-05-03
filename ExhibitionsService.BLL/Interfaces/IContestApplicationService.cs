using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IContestApplicationService
    {
        Task CreateAsync(ContestApplicationDTO entity, ClaimsPrincipal claims);
        Task ConfirmApplicationAsync(int id);
        Task ConfirmWinningAsync(int id);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<ContestApplicationDTO?> GetByIdAsync(int id);
        Task<List<ContestApplicationDTO>> GetAllAsync();
        Task<Tuple<List<ContestApplicationDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);

        Task AddVoteAsync(int applicationId, ClaimsPrincipal claims);
        Task RemoveVoteAsync(int applicationId, ClaimsPrincipal claims);
        Task DetermineWinners();
    }
}
