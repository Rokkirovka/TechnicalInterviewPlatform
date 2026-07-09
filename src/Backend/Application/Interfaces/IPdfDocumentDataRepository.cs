using Domain.Entities;

namespace Application.Interfaces;

public interface IPdfDocumentDataRepository
{
    Task<Candidate?> GetCandidateWithDetailsAsync(int candidateId, CancellationToken ct);
    Task<Interview?> GetInterviewWithDetailsAsync(int interviewId, CancellationToken ct);
}
