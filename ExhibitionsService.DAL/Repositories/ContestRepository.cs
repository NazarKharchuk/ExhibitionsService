using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class ContestRepository : RepositoryBase<Contest>, IContestRepository
    {
        public ContestRepository(ExhibitionContext _db) : base(_db)
        {
        }
    }
}
