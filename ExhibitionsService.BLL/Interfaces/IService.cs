﻿namespace ExhibitionsService.BLL.Interfaces
{
    public interface IService<T> where T : class
    {
        /*Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();*/
        void Dispose();
    }
}
