using Domain.Entities;

namespace Application.Interfaces;

public interface IDeletionLogRepository<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity, int deletedByUserId, string? reason = null);
    Task<IReadOnlyList<DeletionLogBase<TEntity>>> GetByEntityIdAsync(int entityId);
    Task<IReadOnlyList<DeletionLogBase<TEntity>>> GetAllAsync();
}