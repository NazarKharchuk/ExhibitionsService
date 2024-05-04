using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IExhibitionApplicationRepository : IRepository<ExhibitionApplication>
    {
        IQueryable<ExhibitionApplication> GetAllApplicationsWithinfo();
    }
}
