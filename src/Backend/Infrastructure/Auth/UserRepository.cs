using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private readonly DbSet<User> _userDbSet = context.Users;

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _userDbSet
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null);
    }
    
    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _userDbSet
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login == login && u.DeletedAt == null);
    }
}
