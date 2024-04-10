using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IPaintingRepository : IRepository<Painting>
    {
        void AddLike(Painting painting, PaintingLike like);
        void RemoveLike(int paintingId, int profileId);
        Task<IQueryable<Painting>> FindPaintingWithLikes(Func<Painting, bool> predicate);
    }
}
