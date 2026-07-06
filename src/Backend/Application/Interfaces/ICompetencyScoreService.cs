using Application.Dtos;

namespace Application.Interfaces;

public interface ICompetencyScoreService : IBaseService<CompetencyScoreDto, CreateCompetencyScoreRequest, UpdateCompetencyScoreRequest>
{
    Task<IReadOnlyList<CompetencyScoreDto>> GetByInterviewIdAsync(int interviewId);
}