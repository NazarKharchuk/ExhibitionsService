using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class PaintingRepository : RepositoryBase<Painting>, IPaintingRepository
    {
        public PaintingRepository(ExhibitionContext _db) : base(_db)
        {

        }

        public void AddLike(Painting painting, PaintingLike like)
        {
            painting.PaintingLikes.Add(like);
        }

        public void RemoveLike(int paintingId, int profileId)
        {
            Painting? painting = db.Paintings.Include(p => p.PaintingLikes).Where(p => p.PaintingId == paintingId).FirstOrDefault();
            if (painting != null)
            {
                PaintingLike? paintingLike = painting.PaintingLikes.FirstOrDefault(l => l.ProfileId == profileId);
                if(paintingLike != null) painting.PaintingLikes.Remove(paintingLike);
            }
        }

        public async Task<IQueryable<Painting>> FindPaintingWithLikes(Func<Painting, bool> predicate)
        {
            return db.Paintings.Include(p => p.PaintingLikes).Where(predicate).AsQueryable();
        }
    }
}
