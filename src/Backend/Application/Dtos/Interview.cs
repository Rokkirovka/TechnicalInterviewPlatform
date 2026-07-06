using Domain.Enums;

namespace Application.Dtos;

public class InterviewDto : BaseDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public InterviewStatus Status { get; set; }
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public int? DecidedByUserId { get; set; }
    public string? DecidedByUserName { get; set; }
    public List<CompetencyScoreDto> CompetencyScores { get; set; } = new();
}

public class CreateInterviewRequest
{
    public int CandidateId { get; set; }
    public int VacancyId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
}

public class UpdateInterviewRequest
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public InterviewStatus Status { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? DecidedByUserId { get; set; }
}

public class UpdateInterviewStatusRequest
{
    public int Id { get; set; }
    public InterviewStatus Status { get; set; }
    public int? DecidedByUserId { get; set; }
}