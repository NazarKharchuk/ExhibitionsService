using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExhibitionsService.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExhibitionContext db;
        private UserProfileRepository UserProfileRepository;
        private PainterRepository PainterRepository;

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

        public IPainterRepository Painters
        {
            get
            {
                PainterRepository ??= new PainterRepository(db);
                return PainterRepository;
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await db.Database.BeginTransactionAsync();
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
