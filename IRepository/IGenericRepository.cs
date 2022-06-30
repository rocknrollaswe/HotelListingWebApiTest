﻿namespace HotelListing.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id); 
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int? id);
        Task<bool> Exists(int id); 
    }


}
