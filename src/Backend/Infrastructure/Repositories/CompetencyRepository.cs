using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CompetencyRepository(ApplicationDbContext context)
    : BaseRepository<Competency>(context), ICompetencyRepository
{
    public async Task<IReadOnlyList<Competency>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }
    
    public async Task<IReadOnlyList<Competency>> GetByIdsAsync(List<int> ids)
    {
        return await DbSet.Where(c => ids.Contains(c.Id)).ToListAsync();
    }
}