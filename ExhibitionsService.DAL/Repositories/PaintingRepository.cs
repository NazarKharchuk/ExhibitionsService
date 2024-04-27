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

        public void AddItem<T>(ICollection<T> list, T item)
        {
            list.Add(item);
        }

        public void RemoveItem<T>(ICollection<T> list, T item)
        {
            list.Remove(item);
        }

        public async Task<IQueryable<Painting>> GetAllPaintingsWithInfoAsync()
        {
            return db.Paintings
                .Include(p => p.Painter)
                .Include(p => p.Ratings)
                .Include(p => p.PaintingLikes)
                .Include(p => p.Genres)
                .Include(p => p.Styles)
                .Include(p => p.Materials)
                .Include(p => p.Tags)
                .Include(p => p.ExhibitionApplications)
                .Include(p => p.ContestApplications)
                .AsQueryable();
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
    }
}
