using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPainterService : IService<PainterDTO>
    {
        Task CreateAsync(PainterDTO entity);
        Task<PainterDTO> UpdateAsync(PainterDTO entity);
        Task DeleteAsync(int id);
        Task<PainterDTO?> GetByIdAsync(int id);
        Task<List<PainterDTO>> GetAllAsync();
    }
}
