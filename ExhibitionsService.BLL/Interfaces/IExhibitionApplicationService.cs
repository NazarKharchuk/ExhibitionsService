using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IExhibitionApplicationService
    {
        Task CreateAsync(ExhibitionApplicationDTO entity, ClaimsPrincipal claims);
        Task ConfirmApplicationAsync(int id);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<ExhibitionApplicationDTO?> GetByIdAsync(int id);
        Task<List<ExhibitionApplicationDTO>> GetAllAsync();
        Task<Tuple<List<ExhibitionApplicationDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);
    }
}
