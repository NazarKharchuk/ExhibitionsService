using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class PainterRepository : RepositoryBase<Painter>, IPainterRepository
    {
        public PainterRepository(ExhibitionContext _db) : base(_db)
        {
        }

        public IQueryable<Painter> GetAllPaintersWithInfo()
        {
            return db.Painters
                .Include(p => p.UserProfile)
                .Include(p => p.Paintings)
                    .ThenInclude(pg => pg.PaintingLikes)
                .Include(p => p.Paintings)
                    .ThenInclude(pg => pg.Ratings)
                .Include(p => p.Paintings)
                    .ThenInclude(pg => pg.ContestApplications)
                        .ThenInclude(ca => ca.Contest);
        }

        public IQueryable<PaintingLike> GetPainterLikes(int painterId)
        {
            return db.PaintingLikes.Include(pl => pl.Painting).Where(l => l.Painting.PainterId == painterId);
        }

        public IQueryable<PaintingRating> GetPainterRatings(int painterId)
        {
            return db.PaintingRatings.Include(pl => pl.Painting).Where(r => r.Painting.PainterId == painterId);
        }
    }
}
