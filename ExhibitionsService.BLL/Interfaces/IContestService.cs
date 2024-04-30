using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

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
    }
}
