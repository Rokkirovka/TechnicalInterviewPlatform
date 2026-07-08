using Application.Dtos;

namespace Application.Interfaces;

public interface ICandidateService
{
    Task<IReadOnlyList<CandidateDto>> SearchAsync(string? search, bool showArchived);
    Task<IReadOnlyList<CandidateNameDto>> GetNamesAsync();
    Task<CandidateDto> GetByIdAsync(int id);
    Task<CandidateDto> CreateAsync(CreateCandidateRequest request);
    Task<CandidateDto> UpdateAsync(UpdateCandidateRequest request);
    Task ArchiveAsync(int id, string? reason, int archivedByUserId);
    Task RestoreAsync(int id);
}