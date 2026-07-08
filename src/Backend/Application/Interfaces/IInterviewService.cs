using Application.Dtos;

namespace Application.Interfaces;

public interface IInterviewService
{
    Task<IReadOnlyList<InterviewDto>> SearchAsync(string? search, bool showArchived);
    Task<InterviewDto> GetByIdAsync(int id);
    Task<InterviewDto> CreateAsync(CreateInterviewRequest request);
    Task<InterviewDto> UpdateAsync(UpdateInterviewRequest request, int userId);
    Task MarkPassedAsync(int id);
    Task SetDecisionAsync(int id, string decision);
    Task ArchiveAsync(int id, string? reason, int archivedByUserId);
    Task RestoreAsync(int id);
}