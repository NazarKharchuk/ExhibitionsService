﻿using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExhibitionContext db;
        private UserProfileRepository UserProfileRepository;

        public UnitOfWork(ExhibitionContext context)
        {
            db = context;
        }

        public IUserProfileRepository UserProfiles
        {
            get
            {
                UserProfileRepository ??= new UserProfileRepository(db);
                return UserProfileRepository;
            }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
