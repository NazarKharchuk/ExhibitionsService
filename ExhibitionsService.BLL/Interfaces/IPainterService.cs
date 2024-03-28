using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPainterService : IService<PainterDTO>
    {
        Task CreateAsync(PainterDTO entity);

        Task<List<PainterDTO>> GetAllAsync();
    }
}
