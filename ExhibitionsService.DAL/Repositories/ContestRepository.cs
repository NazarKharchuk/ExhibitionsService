using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Repositories
{
    public class ContestRepository : RepositoryBase<Contest>, IContestRepository
    {
        public ContestRepository(ExhibitionContext _db) : base(_db)
        {
        }

        public IQueryable<Contest> GetAllContestsWithInfo()
        {
            return db.Contests
                .Include(c => c.Applications)
                    .ThenInclude(ca => ca.Voters)
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
