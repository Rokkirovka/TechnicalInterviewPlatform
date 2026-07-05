using Domain.Entities;

namespace Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdIncludingDeletedAsync(int id);
    Task<IReadOnlyList<T>> AllAsync();
    Task<IReadOnlyList<T>> AllAliveAsync();
    Task<IReadOnlyList<T>> AllDeletedAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}