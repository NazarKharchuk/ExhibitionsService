﻿using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        Task CreateAsync(UserProfile item);
    }
}
