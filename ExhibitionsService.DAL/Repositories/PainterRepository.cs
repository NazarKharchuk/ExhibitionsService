using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class PainterRepository : RepositoryBase<Painter>, IPainterRepository
    {
        public PainterRepository(ExhibitionContext _db) : base(_db)
        {
        }
    }
}
