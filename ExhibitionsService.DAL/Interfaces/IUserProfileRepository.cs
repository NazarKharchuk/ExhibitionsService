﻿using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        Task<Tuple<User?, UserProfile?>> GetUserAndProfileByIdAsync(int profileId);
        Task<IQueryable<UserProfile>> GetAllProfilesWithUsersAsync();
        Task DeleteRelatedInfo(int profileId);
    }
}
