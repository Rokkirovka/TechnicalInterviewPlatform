using Domain.Entities;

namespace Application.Interfaces;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<IReadOnlyList<Candidate>> SearchAsync(string? search, bool showArchived);
}