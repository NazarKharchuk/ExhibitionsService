using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Interfaces;
using System.Linq.Expressions;

namespace ExhibitionsService.DAL.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ExhibitionContext db;

        public RepositoryBase(ExhibitionContext _db)
        {
            db = _db;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await db.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            db.Set<T>().Update(entity);
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            T? entity = await GetByIdAsync(id);
            if (entity != null) db.Set<T>().Remove(entity);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }

        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            return db.Set<T>().AsQueryable();
        }

        public virtual async Task<IQueryable<T>> FindAsync(Func<T, bool> predicate)
        {
            return db.Set<T>().Where(predicate).AsQueryable();
        }
    }
}
