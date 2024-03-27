using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Interfaces;

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
            await db.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            db.Set<T>().Update(entity);
            await db.SaveChangesAsync();
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
    }
}
