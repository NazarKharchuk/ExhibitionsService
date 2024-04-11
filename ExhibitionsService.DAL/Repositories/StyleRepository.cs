using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class StyleRepository : RepositoryBase<Style>, IStyleRepository
    {
        public StyleRepository(ExhibitionContext _db) : base(_db)
        {

        }
    }
}
