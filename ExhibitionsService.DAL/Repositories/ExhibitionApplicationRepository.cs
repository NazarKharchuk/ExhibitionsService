using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class ExhibitionApplicationRepository : RepositoryBase<ExhibitionApplication>, IExhibitionApplicationRepository
    {
        public ExhibitionApplicationRepository(ExhibitionContext _db) : base(_db)
        {

        }
    }
}
