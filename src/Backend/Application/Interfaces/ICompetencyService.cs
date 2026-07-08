using Application.Dtos;

namespace Application.Interfaces;

public interface ICompetencyService
{
    Task<IReadOnlyList<CompetencyDto>> GetAllAsync();
    Task<CompetencyDto> CreateAsync(CreateCompetencyRequest request);
}