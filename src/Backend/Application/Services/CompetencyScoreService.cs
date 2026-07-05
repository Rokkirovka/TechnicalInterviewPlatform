using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CompetencyScoreService(
    IRepository<CompetencyScore> repository,
    IDeletionLogRepository<CompetencyScore> deletionLogRepository,
    IRepository<Interview> interviewRepository,
    IRepository<Competency> competencyRepository,
    IMapper mapper)
    : BaseService<CompetencyScore, CompetencyScoreDto, CreateCompetencyScoreRequest, UpdateCompetencyScoreRequest>(
            repository,
            deletionLogRepository,
            mapper),
        ICompetencyScoreService
{
    public async Task<IReadOnlyList<CompetencyScoreDto>> GetByInterviewIdAsync(int interviewId)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {interviewId} не найдено");

        var scores = await Repository.AllAliveAsync();
        var filteredScores = scores.Where(s => s.InterviewId == interviewId).ToList();
        
        return Mapper.Map<IReadOnlyList<CompetencyScoreDto>>(filteredScores);
    }

    public override async Task<CompetencyScoreDto> CreateAsync(CreateCompetencyScoreRequest request)
    {
        var interview = await interviewRepository.GetByIdAsync(request.InterviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {request.InterviewId} не найдено");

        var competency = await competencyRepository.GetByIdAsync(request.CompetencyId);
        if (competency == null)
            throw new Exception($"Компетенция с id {request.CompetencyId} не найдена");

        var score = Mapper.Map<CompetencyScore>(request);
        var result = await Repository.AddAsync(score);
        return Mapper.Map<CompetencyScoreDto>(result);
    }
}