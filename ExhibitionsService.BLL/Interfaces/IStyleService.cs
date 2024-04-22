using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IStyleService : IService<StyleDTO>
    {
        Task CreateAsync(StyleDTO entity);
        Task<StyleDTO> UpdateAsync(StyleDTO entity);
        Task DeleteAsync(int id);
        Task<StyleDTO?> GetByIdAsync(int id);
        Task<List<StyleDTO>> GetAllAsync();
        Task<Tuple<List<StyleDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);
    }
}
