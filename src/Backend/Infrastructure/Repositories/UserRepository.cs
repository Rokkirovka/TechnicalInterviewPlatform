using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<IReadOnlyList<User>> SearchAsync(string? search, bool showArchived)
    {
        var query = DbSet.Include(u => u.Roles).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) 
            query = query.Where(u => EF.Functions.ILike(u.FullName, $"%{search}%"));

        query = showArchived
            ? query.Where(u => u.DeletedAt != null)
            : query.Where(u => u.DeletedAt == null);

        return await query.ToListAsync();
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await DbSet
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());
    }

    public async Task<User?> GetWithRolesAsync(int id)
    {
        return await DbSet
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}