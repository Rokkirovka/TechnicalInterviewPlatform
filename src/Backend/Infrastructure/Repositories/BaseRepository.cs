using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BaseRepository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await context.SaveChangesAsync();
    }
}