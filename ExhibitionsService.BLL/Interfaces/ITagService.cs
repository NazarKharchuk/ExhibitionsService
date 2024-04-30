using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface ITagService : IService<TagDTO>
    {
        Task CreateAsync(TagDTO entity);
        Task<TagDTO> UpdateAsync(TagDTO entity);
        Task DeleteAsync(int id);
        Task<TagDTO?> GetByIdAsync(int id);
        Task<List<TagDTO>> GetAllAsync();
        Task<Tuple<List<TagDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);
    }
}
