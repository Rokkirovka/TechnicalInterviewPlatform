using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<IReadOnlyList<Role>> GetAllAsync()
    {
        return await context.Roles.ToListAsync();
    }
}