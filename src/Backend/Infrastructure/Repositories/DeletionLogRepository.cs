using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeletionLogRepository<TEntity>(ApplicationDbContext context) : IDeletionLogRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly DbSet<DeletionLogBase<TEntity>> _dbSet = context.Set<DeletionLogBase<TEntity>>();

    public async Task AddAsync(TEntity entity, int deletedByUserId, string? reason = null)
    {
        var log = new DeletionLogBase<TEntity>
        {
            EntityId = entity.Id,
            DeletedByUserId = deletedByUserId,
            DeletionReason = reason ?? "Причина не указана",
            DeletedAt = DateTime.UtcNow,
            Entity = entity
        };

        await _dbSet.AddAsync(log);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<DeletionLogBase<TEntity>>> GetByEntityIdAsync(int entityId)
    {
        return await _dbSet
            .Where(l => l.EntityId == entityId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<DeletionLogBase<TEntity>>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
}