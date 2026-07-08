using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SkillRepository(ApplicationDbContext context) : BaseRepository<Skill>(context), ISkillRepository
{
    public async Task<IReadOnlyList<Skill>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IReadOnlyList<Skill>> GetByNamesAsync(List<string> names)
    {
        var lowerNames = names.Select(n => n.ToLower()).ToList();
        return await DbSet.Where(s => lowerNames.Contains(s.Name.ToLower())).ToListAsync();
    }
}