using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private readonly DbSet<User> _userDbSet = context.Users;

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _userDbSet.FirstOrDefaultAsync(u => u.Login == login && u.DeletedAt == null);
    }
}
