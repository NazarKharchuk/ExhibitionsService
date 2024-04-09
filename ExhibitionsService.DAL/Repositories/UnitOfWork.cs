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
        private TagRepository TagRepository;
        private PaintingRepository PaintingRepository;
        private ImageRepository ImageRepository;
        private PaintingRatingRepository PaintingRatingRepository;

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

        public ITagRepository Tags
        {
            get
            {
                TagRepository ??= new TagRepository(db);
                return TagRepository;
            }
        }

        public IPaintingRepository Paintings
        {
            get
            {
                PaintingRepository ??= new PaintingRepository(db);
                return PaintingRepository;
            }
        }

        public IImageRepository Images
        {
            get
            {
                ImageRepository ??= new ImageRepository(Path.Combine(Environment.CurrentDirectory, "wwwroot"));
                return ImageRepository;
            }
        }

        public IPaintingRatingRepository PaintingRatings
        {
            get
            {
                PaintingRatingRepository ??= new PaintingRatingRepository(db);
                return PaintingRatingRepository;
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
