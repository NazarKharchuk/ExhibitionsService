using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class UserProfileRepository : RepositoryBase<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(ExhibitionContext _db) : base(_db) { }

        public async Task<Tuple<User?, UserProfile?>> GetUserAndProfileByIdAsync(int profileId)
        {
            UserProfile profile = db.UserProfiles.Include(p => p.User).Where(p => p.ProfileId == profileId).FirstOrDefault();

            if (profile == null) return new Tuple<User?, UserProfile?>(null, null);
            else return new Tuple<User?, UserProfile?>(profile.User, profile);
        }

        public async Task<IQueryable<UserProfile>> GetAllProfilesWithUsersAsync()
        {
            return db.UserProfiles.Include(p => p.User).AsQueryable();
        }

        public async Task DeleteRelatedInfo(int profileId)
        {
            var likes = await db.PaintingLikes.Where(pl => pl.ProfileId == profileId).ToListAsync();
            db.PaintingLikes.RemoveRange(likes);

            var votedApplications = await db.ContestApplications.Include(ca => ca.Voters).
                Where(ca => ca.Voters.Any(v => v.ProfileId == profileId)).ToListAsync();
            foreach( var application in votedApplications )
            {
                UserProfile? profile = application.Voters.FirstOrDefault(l => l.ProfileId == profileId);
                if (profile != null) application.Voters.Remove(profile);
            }

            var ratings = await db.PaintingRatings.Where(pr => pr.ProfileId == profileId).ToListAsync();
            db.PaintingRatings.RemoveRange(ratings);
        }
    }
}
