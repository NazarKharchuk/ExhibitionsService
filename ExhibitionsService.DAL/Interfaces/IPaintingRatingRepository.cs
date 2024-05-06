using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IPaintingRatingRepository : IRepository<PaintingRating>
    {
        IQueryable<PaintingRating> GetAllPaintingRatingsWithInfo();
    }
}
