using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IContestApplicationRepository : IRepository<ContestApplication>
    {
        void AddVote(ContestApplication application, UserProfile profile);
        void RemoveVote(int applicationId, int profileId);
        Task<IQueryable<ContestApplication>> FindApplicationsWithVoters(Func<ContestApplication, bool> predicate);
    }
}
