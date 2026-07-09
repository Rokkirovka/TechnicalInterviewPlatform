using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class InterviewService(
    IInterviewRepository interviewRepository,
    IVacancyRepository vacancyRepository,
    ICandidateRepository candidateRepository,
    IUserRepository userRepository,
    IDeletionLogRepository<Interview> deletionLogRepository,
    IMapper mapper,
    ILogger<InterviewService> logger) : IInterviewService
{
    public async Task<IReadOnlyList<InterviewDto>> SearchAsync(string? search, bool showArchived)
    {
        var interviews = await interviewRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<InterviewDto>>(interviews);
    }

    public async Task<IReadOnlyList<InterviewDto>> GetByCandidateIdAsync(int candidateId)
    {
        var interviews = await interviewRepository.GetByCandidateIdAsync(candidateId);
        return mapper.Map<IReadOnlyList<InterviewDto>>(interviews);
    }

    public async Task<InterviewDto> GetByIdAsync(int id)
    {
        var interview = await interviewRepository.GetWithDetailsAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        return mapper.Map<InterviewDto>(interview);
    }

    public async Task<InterviewDto> CreateAsync(CreateInterviewRequest request, int createdByUserId)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(request.VacancyId);
        if (vacancy == null) throw new KeyNotFoundException($"Вакансия с id {request.VacancyId} не найдена");
        
        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate == null) throw new KeyNotFoundException($"Кандидат с id {request.VacancyId} не найден");
        
        var creator = await userRepository.GetByIdAsync(createdByUserId);
        if (creator == null) throw new KeyNotFoundException($"Пользователь с id {request.VacancyId} не найден. Вы вообще кто?");

        var interview = new Interview
        {
            CandidateId = request.CandidateId,
            Candidate = candidate,
            VacancyId = request.VacancyId,
            ScheduledAt = request.DateTime,
            Status = InterviewStatus.Scheduled,
            CreatedByUserId = createdByUserId,
            CreatedByUser = creator,
            CreatedAt = DateTime.UtcNow,
            InterviewStages = request.Stages
                .Select(s => new InterviewStage
                {
                    StageName = s.Name,
                    Duration = s.Duration,
                    Description = s.Description,
                    OrderNumber = s.StageNumber
                })
                .ToList(),
            CompetencyScores = vacancy.VacancyCompetencies
                .Select(vc => new CompetencyScore
                {
                    CompetencyId = vc.CompetencyId,
                    Score = 0,
                    Comment = string.Empty
                })
                .ToList()
        };

        await interviewRepository.AddAsync(interview);
        var result = mapper.Map<InterviewDto>(interview);
        
        logger.LogInformation("интервью {InterviewId} было создано", result.Id);
        
        return result;
    }

    public async Task<InterviewDto> UpdateAsync(int id, UpdateInterviewRequest request, int userId)
    {
        var interview = await interviewRepository.GetWithDetailsAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");

        interview.ScheduledAt = request.DateTime;

        if (request.Matrix.Count != 0)
        {
            foreach (var scoreUpdate in request.Matrix)
            {
                var score = interview.CompetencyScores.FirstOrDefault(cs => cs.Id == scoreUpdate.Id);
                if (score != null) score.Score = scoreUpdate.Score;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            interview.Comments.Add(new Comment
            {
                Content = request.Comment,
                AuthorId = userId
            });
        }

        await interviewRepository.UpdateAsync(interview);
        var result = mapper.Map<InterviewDto>(interview);
        
        logger.LogInformation("интервью {InterviewId} было обновлено", result.Id);
        
        return result;
    }

    public async Task MarkPassedAsync(int id)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.Status = InterviewStatus.Passed;
        await interviewRepository.UpdateAsync(interview);
        
        logger.LogInformation("интервью {InterviewId} было помечено законченным", interview.Id);
    }

    public async Task SetDecisionAsync(int id, string decision)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");

        interview.Status = decision.ToLower() switch
        {
            "approved" => InterviewStatus.Approved,
            "rejected" => InterviewStatus.Rejected,
            "to_next_stage" => InterviewStatus.ToNextStage,
            _ => throw new ArgumentException($"Некорректное решение: {decision}")
        };

        await interviewRepository.UpdateAsync(interview);
        
        logger.LogInformation(
            "интервью {InterviewId} получило решение: {Interview}", 
            interview.Id, 
            interview.Status.ToString());
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.DeletedAt = DateTime.UtcNow;
        await interviewRepository.UpdateAsync(interview);
        await deletionLogRepository.AddAsync(interview, archivedByUserId, reason);
        
        logger.LogInformation("интервью {InterviewId} было архивировано", interview.Id);
    }

    public async Task RestoreAsync(int id)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.DeletedAt = null;
        await interviewRepository.UpdateAsync(interview);
        
        logger.LogInformation("интервью {InterviewId} было восстановлено из архива", interview.Id);
    }
}