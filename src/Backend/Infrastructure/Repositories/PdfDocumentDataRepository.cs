using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PdfDocumentDataRepository(ApplicationDbContext context) : IPdfDocumentDataRepository
{
    public async Task<Candidate?> GetCandidateWithDetailsAsync(int candidateId, CancellationToken ct)
    {
        return await context.Candidates
            .Include(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == candidateId && c.DeletedAt == null, ct);
    }

    public async Task<Interview?> GetInterviewWithDetailsAsync(int interviewId, CancellationToken ct)
    {
        return await context.Interviews
            .Include(i => i.Candidate)
            .ThenInclude(c => c.CandidateSkills)
            .ThenInclude(cs => cs.Skill)
            .Include(i => i.Vacancy)
            .Include(i => i.CreatedByUser)
            .Include(i => i.AssignedToUser)
            .Include(i => i.DecidedByUser)
            .Include(i => i.CompetencyScores)
            .ThenInclude(cs => cs.Competency)
            .Include(i => i.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(i => i.Id == interviewId && i.DeletedAt == null, ct);
    }
}
