using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface ITagService : IService<TagDTO>
    {
        Task CreateAsync(TagDTO entity);
        Task<TagDTO> UpdateAsync(TagDTO entity);
        Task DeleteAsync(int id);
        Task<TagDTO?> GetByIdAsync(int id);
        Task<List<TagDTO>> GetAllAsync();
    }
}
