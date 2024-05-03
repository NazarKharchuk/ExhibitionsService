using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IContestService
    {
        Task<ContestDTO> CreateAsync(ContestDTO entity);
        Task<ContestDTO> UpdateAsync(ContestDTO entity);
        Task DeleteAsync(int id);
        Task<ContestDTO?> GetByIdAsync(int id);
        Task<List<ContestDTO>> GetAllAsync();
        Task<ContestInfoDTO> GetByIdWithInfoAsync(int id);
        Task<Tuple<List<ContestInfoDTO>, int>> GetPageContestInfoAsync(PaginationRequestDTO pagination);
        Task AddTagAsync(int contestId, int tagId);
        Task RemoveTagAsync(int contestId, int tagId);

        Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPageContestApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId);
        Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPageContestNotConfirmedApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId);
        Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetPainterContestSubmissionsAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId);
        Task<Tuple<List<ContestApplicationInfoDTO>, int>> GetUserContestVotesAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int contestId);
    }
}
