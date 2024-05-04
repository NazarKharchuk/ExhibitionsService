using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class ExhibitionRepository: RepositoryBase<Exhibition>, IExhibitionRepository
    {
        public ExhibitionRepository(ExhibitionContext _db) : base(_db)
        {

        }

        public IQueryable<Exhibition> GetAllExhibitionsWithInfo()
        {
            return db.Exhibitions
                .Include(c => c.Applications)
                .Include(c => c.Tags);
        }

        public void AddItem<T>(ICollection<T> list, T item)
        {
            list.Add(item);
        }

        public void RemoveItem<T>(ICollection<T> list, T item)
        {
            list.Remove(item);
        }
    }
}
