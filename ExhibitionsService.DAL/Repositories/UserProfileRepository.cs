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
    }
}
