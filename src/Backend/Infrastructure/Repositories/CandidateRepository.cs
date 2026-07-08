using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CandidateRepository(ApplicationDbContext context)
    : BaseRepository<Candidate>(context), ICandidateRepository
{
    public async Task<IReadOnlyList<Candidate>> SearchAsync(string? search, bool showArchived)
    {
        var query = search is not null 
            ? DbSet.Where(c => EF.Functions.ILike(c.FullName, $"%{search}%")) 
            : DbSet;
        
        query = showArchived
            ? query.Where(c => c.DeletedAt != null)
            : query.Where(c => c.DeletedAt == null);
        
        return await query.ToListAsync();
    }
}