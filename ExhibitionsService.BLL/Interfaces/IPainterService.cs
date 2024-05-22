using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPainterService : IService<PainterDTO>
    {
        Task CreateAsync(PainterDTO entity, ClaimsPrincipal claims);
        Task<PainterDTO> UpdateAsync(PainterDTO entity, ClaimsPrincipal claims);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<PainterDTO?> GetByIdAsync(int id);
        Task<List<PainterDTO>> GetAllAsync();
        Task<PainterInfoDTO> GetByIdWithInfoAsync(int id);
        Task<Tuple<List<PainterInfoDTO>, int>> GetPagePainterInfoAsync(PaintersFiltrationPaginationRequestDTO filters);

        Task<StatisticsResponseDTO<StatisticsLikesValueDTO>?> GetLikesStatistics
            (int painterId, DateTime periodStartDate, string periodSize);
        Task<StatisticsResponseDTO<StatisticsRatingsValueDTO>?> GetRatingsStatistics
            (int painterId, DateTime periodStartDate, string periodSize);
    }
}
