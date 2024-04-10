using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class ContestApplicationRepository : RepositoryBase<ContestApplication>, IContestApplicationRepository
    {
        public ContestApplicationRepository(ExhibitionContext _db) : base(_db)
        {
        }
    }
}
