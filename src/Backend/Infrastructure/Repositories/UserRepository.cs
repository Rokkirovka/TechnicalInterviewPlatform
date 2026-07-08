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

    public override async Task UpdateAsync(User entity)
    {
        var dbEntity = await DbSet.FindAsync(entity.Id);
        if (dbEntity == null) return;
        dbEntity.UpdatedAt = DateTime.UtcNow;

        dbEntity.FirstName = entity.FirstName;
        dbEntity.MiddleName = entity.MiddleName;
        dbEntity.LastName = entity.LastName;
        dbEntity.Login = entity.Login;

        if (!string.IsNullOrEmpty(entity.PasswordHash))
        {
            dbEntity.PasswordHash = entity.PasswordHash;
        }

        foreach (var role in dbEntity.Roles)
        {
            if (!entity.Roles.Any(r => r.Name == role.Name))
            {
                dbEntity.Roles.Remove(role);
            }
        }
        foreach (var role in entity.Roles)
        {
            if (!dbEntity.Roles.Any(r => r.Name == role.Name))
            {
                dbEntity.Roles.Add(role);
            }
        }

        await SaveChangesAsync();
    }
}