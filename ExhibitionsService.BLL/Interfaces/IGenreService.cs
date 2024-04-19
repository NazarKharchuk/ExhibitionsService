using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IGenreService : IService<GenreDTO>
    {
        Task CreateAsync(GenreDTO entity);
        Task<GenreDTO> UpdateAsync(GenreDTO entity);
        Task DeleteAsync(int id);
        Task<GenreDTO?> GetByIdAsync(int id);
        Task<List<GenreDTO>> GetAllAsync();
        Task<Tuple<List<GenreDTO>, int>> GetPageAsync(PaginationRequestDTO pagination);
    }
}
