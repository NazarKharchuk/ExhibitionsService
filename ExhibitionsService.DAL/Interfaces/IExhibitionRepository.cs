using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IExhibitionRepository : IRepository<Exhibition>
    {
        IQueryable<Exhibition> GetAllExhibitionsWithInfo();
        void AddItem<T>(ICollection<T> list, T item);
        void RemoveItem<T>(ICollection<T> list, T item);
    }
}
