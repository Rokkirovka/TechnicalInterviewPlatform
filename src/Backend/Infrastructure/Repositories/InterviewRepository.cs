using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterviewRepository(ApplicationDbContext context)
    : BaseRepository<Interview>(context), IInterviewRepository
{
    public async Task<IReadOnlyList<Interview>> SearchAsync(string? search, bool showArchived)
    {
        var query = DbSet
            .Include(i => i.Candidate)
            .Include(i => i.Vacancy)
            .AsQueryable();

        if (search is not null) 
            query = query.Where(i =>
                EF.Functions.ILike(i.Candidate.FirstName, $"%{search}%") ||
                EF.Functions.ILike(i.Candidate.LastName, $"%{search}%") ||
                EF.Functions.ILike(i.Candidate.MiddleName, $"%{search}%"));

        query = showArchived
            ? query.Where(i => i.DeletedAt != null)
            : query.Where(i => i.DeletedAt == null);

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Interview>> GetByCandidateIdAsync(int candidateId)
    {
        return await DbSet
            .Include(i => i.Candidate)
            .Include(i => i.Vacancy)
            .Where(i => i.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<Interview?> GetWithDetailsAsync(int id)
    {
        // Этот код можно сократить?
        // БОЛЬШЕ КОДА БОГУ КОДА
        return await DbSet
            .Include(i => i.Candidate)
            .Include(i => i.Vacancy)
            .Include(i => i.CreatedByUser)
            .Include(i => i.InterviewStages)
            .Include(i => i.CompetencyScores)
                .ThenInclude(cs => cs.Competency)
            .Include(i => i.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}