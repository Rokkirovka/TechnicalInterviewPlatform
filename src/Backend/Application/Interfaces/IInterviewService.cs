using Application.Dtos;

namespace Application.Interfaces;

public interface IInterviewService
{
    Task<IReadOnlyList<InterviewDto>> SearchAsync(string? search, bool showArchived);
    Task<IReadOnlyList<InterviewDto>> GetByCandidateIdAsync(int candidateId);
    Task<InterviewDto> GetByIdAsync(int id);
    Task<InterviewDto> CreateAsync(CreateInterviewRequest request, int createdByUserId);
    Task<InterviewDto> UpdateAsync(int id, UpdateInterviewRequest request, int userId);
    Task MarkPassedAsync(int id);
    Task SetDecisionAsync(int id, string decision);
    Task ArchiveAsync(int id, string? reason, int archivedByUserId);
    Task RestoreAsync(int id);
}