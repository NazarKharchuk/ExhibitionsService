using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IPainterRepository : IRepository<Painter>
    {
        IQueryable<Painter> GetAllPaintersWithInfo();
    }
}
