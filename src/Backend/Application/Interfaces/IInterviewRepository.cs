using Domain.Entities;

namespace Application.Interfaces;

public interface IInterviewRepository : IRepository<Interview>
{
    Task<IReadOnlyList<Interview>> SearchAsync(string? search, bool showArchived);
    Task<Interview?> GetWithDetailsAsync(int id);
    Task<IReadOnlyList<Interview>> GetByCandidateIdAsync(int candidateId);
}