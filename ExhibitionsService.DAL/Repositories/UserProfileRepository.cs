using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ExhibitionContext db;

        public UserProfileRepository(ExhibitionContext _db)
        {
            db = _db;
        }

        public async Task CreateAsync(UserProfile item)
        {
            await db.UserProfiles.AddAsync(item);
        }
    }
}
