using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IExhibitionService
    {
        Task<ExhibitionDTO> CreateAsync(ExhibitionDTO entity);
        Task<ExhibitionDTO> UpdateAsync(ExhibitionDTO entity);
        Task DeleteAsync(int id);
        Task<ExhibitionDTO?> GetByIdAsync(int id);
        Task<List<ExhibitionDTO>> GetAllAsync();
        Task<ExhibitionInfoDTO> GetByIdWithInfoAsync(int id);
        Task<Tuple<List<ExhibitionInfoDTO>, int>> GetPageExhibitionInfoAsync(ExhibitionFiltrationPaginationRequestDTO filters);
        Task AddTagAsync(int exhibitionId, int tagId);
        Task RemoveTagAsync(int exhibitionId, int tagId);

        Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPageExhibitionApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId);
        Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPageExhibitionNotConfirmedApplicationInfoAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId);
        Task<Tuple<List<ExhibitionApplicationInfoDTO>, int>> GetPainterExhibitionSubmissionsAsync
            (PaginationRequestDTO pagination, ClaimsPrincipal claims, int exhibitionId);
    }
}
