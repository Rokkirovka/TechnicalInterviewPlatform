using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VacancyRepository(ApplicationDbContext context)
    : BaseRepository<Vacancy>(context), IVacancyRepository
{
    public async Task<Vacancy?> GetWithCompetenciesAsync(int id)
    {
        return await DbSet
            .Include(v => v.VacancyCompetencies)
            .ThenInclude(vc => vc.Competency)
            .FirstOrDefaultAsync(v => v.Id == id);
    }
    
    public async Task<IReadOnlyList<Vacancy>> SearchAsync(string? search, bool showArchived)
    {
        var query = search is not null 
            ? DbSet.Where(v => EF.Functions.ILike(v.Title, $"%{search}%")) 
            : DbSet;

        query = showArchived
            ? query.Where(v => v.DeletedAt != null)
            : query.Where(v => v.DeletedAt == null);

        return await query.ToListAsync();
    }
}