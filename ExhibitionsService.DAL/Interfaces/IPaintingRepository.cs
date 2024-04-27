using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IPaintingRepository : IRepository<Painting>
    {
        void AddItem<T>(ICollection<T> list, T item);
        void RemoveItem<T>(ICollection<T> list, T item);
        Task<IQueryable<Painting>> GetAllPaintingsWithInfoAsync();
        void AddLike(Painting painting, PaintingLike like);
        void RemoveLike(int paintingId, int profileId);
    }
}
