using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class PaintingRatingRepository : RepositoryBase<PaintingRating>, IPaintingRatingRepository
    {
        public PaintingRatingRepository(ExhibitionContext _db) : base(_db) { }

        public IQueryable<PaintingRating> GetAllPaintingRatingsWithInfo()
        {
            return db.PaintingRatings
                .Include(p => p.UserProfile);
        }
    }
}
