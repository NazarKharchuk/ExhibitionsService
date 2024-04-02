using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class PaintingRepository : RepositoryBase<Painting>, IPaintingRepository
    {
        public PaintingRepository(ExhibitionContext _db) : base(_db)
        {

        }
    }
}
