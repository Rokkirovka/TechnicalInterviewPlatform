using Application.Dtos;

namespace Application.Interfaces;

public interface IVacancyService
{
    Task<IReadOnlyList<VacancyDto>> SearchAsync(string? search, bool showArchived);
    Task<VacancyDto> GetByIdAsync(int id);
    Task<VacancyDto> CreateAsync(CreateVacancyRequest request);
    Task<VacancyDto> UpdateAsync(int id, UpdateVacancyRequest request);
    Task ArchiveAsync(int id, string? reason, int archivedByUserId);
    Task RestoreAsync(int id);
}