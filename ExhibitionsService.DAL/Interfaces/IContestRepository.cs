using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IContestRepository : IRepository<Contest>
    {
        IQueryable<Contest> GetAllContestsWithInfo();
        void AddItem<T>(ICollection<T> list, T item);
        void RemoveItem<T>(ICollection<T> list, T item);
    }
}
