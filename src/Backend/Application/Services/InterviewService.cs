using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class InterviewService(
    IInterviewRepository interviewRepository,
    IVacancyRepository vacancyRepository,
    IDeletionLogRepository<Interview> deletionLogRepository,
    IMapper mapper) : IInterviewService
{
    public async Task<IReadOnlyList<InterviewDto>> SearchAsync(string? search, bool showArchived)
    {
        var interviews = await interviewRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<InterviewDto>>(interviews);
    }

    public async Task<InterviewDto> GetByIdAsync(int id)
    {
        var interview = await interviewRepository.GetWithDetailsAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        return mapper.Map<InterviewDto>(interview);
    }

    public async Task<InterviewDto> CreateAsync(CreateInterviewRequest request)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(request.VacancyId);
        if (vacancy == null) throw new KeyNotFoundException($"Вакансия с id {request.VacancyId} не найдена");

        var interview = new Interview
        {
            CandidateId = request.CandidateId,
            VacancyId = request.VacancyId,
            ScheduledAt = request.DateTime,
            Status = InterviewStatus.Scheduled,
            CreatedByUserId = request.CreatedByUserId,
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
        return mapper.Map<InterviewDto>(interview);
    }

    public async Task<InterviewDto> UpdateAsync(UpdateInterviewRequest request, int userId)
    {
        var interview = await interviewRepository.GetWithDetailsAsync(request.Id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {request.Id} не найдено");

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
        return mapper.Map<InterviewDto>(interview);
    }

    public async Task MarkPassedAsync(int id)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.Status = InterviewStatus.Passed;
        await interviewRepository.UpdateAsync(interview);
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
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.DeletedAt = DateTime.UtcNow;
        await interviewRepository.UpdateAsync(interview);
        await deletionLogRepository.AddAsync(interview, archivedByUserId, reason);
    }

    public async Task RestoreAsync(int id)
    {
        var interview = await interviewRepository.GetByIdAsync(id);
        if (interview == null) throw new KeyNotFoundException($"Собеседование с id {id} не найдено");
        interview.DeletedAt = null;
        await interviewRepository.UpdateAsync(interview);
    }
}