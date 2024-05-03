using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class ContestApplicationRepository : RepositoryBase<ContestApplication>, IContestApplicationRepository
    {
        public ContestApplicationRepository(ExhibitionContext _db) : base(_db)
        {
        }

        public void AddVote(ContestApplication application, UserProfile profile)
        {
            application.Voters.Add(profile);
        }

        public void RemoveVote(int applicationId, int profileId)
        {
            ContestApplication? application = db.ContestApplications.Include(ca => ca.Voters).Where(ca => ca.ApplicationId == applicationId).FirstOrDefault();
            if (application != null)
            {
                UserProfile? profile = application.Voters.FirstOrDefault(l => l.ProfileId == profileId);
                if (profile != null) application.Voters.Remove(profile);
            }
        }

        public IQueryable<ContestApplication> GetAllApplicationsWithinfo()
        {
            return db.ContestApplications.Include(ca => ca.Voters).Include(ca => ca.Contest);
        }
    }
}
