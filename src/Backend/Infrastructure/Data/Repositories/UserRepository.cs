using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct = default)
    {
        return await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login == login, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateRolesAsync(int id, List<Role> roles, CancellationToken ct = default)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        
        if (user == null)
        {
            return;
        }
        
        var roleIds = roles.Select(r => r.Id).ToList();
        
        var rolesFromDb = await context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync(ct);
        
        user.Roles.Clear();
        foreach (var role in rolesFromDb)
        {
            user.Roles.Add(role);
        }
        
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await context.Users.FindAsync([id], ct);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync(ct);
        }
    }
}