using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IMaterialService : IService<MaterialDTO>
    {
        Task CreateAsync(MaterialDTO entity);
        Task<MaterialDTO> UpdateAsync(MaterialDTO entity);
        Task DeleteAsync(int id);
        Task<MaterialDTO?> GetByIdAsync(int id);
        Task<List<MaterialDTO>> GetAllAsync();
        Task<Tuple<List<MaterialDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);
    }
}
