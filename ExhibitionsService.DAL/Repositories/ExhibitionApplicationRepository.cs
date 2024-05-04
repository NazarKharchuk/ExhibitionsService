using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class ExhibitionApplicationRepository : RepositoryBase<ExhibitionApplication>, IExhibitionApplicationRepository
    {
        public ExhibitionApplicationRepository(ExhibitionContext _db) : base(_db)
        {

        }

        public IQueryable<ExhibitionApplication> GetAllApplicationsWithinfo()
        {
            return db.ExhibitionApplications.Include(ea => ea.Exhibition);
        }
    }
}
