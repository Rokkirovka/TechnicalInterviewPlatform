namespace Application.Dtos;

public class InterviewDto : BaseDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "unknown";
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public int? DecidedByUserId { get; set; }
    public string? DecidedByUserName { get; set; }
    
    public List<InterviewStageDto> Stages { get; set; } = [];
    public List<CompetencyScoreDto> Matrix { get; set; } = [];
    public List<CommentDto> Comments { get; set; } = [];
    public bool Archived { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class InterviewStageDto
{
    public int Id { get; set; }
    public int StageNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
}

public class CompetencyScoreDto
{
    public int Id { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class CreateInterviewRequest
{
    public int CandidateId { get; set; }
    public int VacancyId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int CreatedByUserId { get; set; }
    public List<CreateInterviewStageRequest> Stages { get; set; } = new();
}

public class CreateInterviewStageRequest
{
    public int StageNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
}

public class UpdateInterviewRequest
{
    public DateTime ScheduledAt { get; set; }
    public List<UpdateCompetencyScoreRequest> Matrix { get; set; } = new();
    public string? Comment { get; set; }
}

public class UpdateCompetencyScoreRequest
{
    public int Id { get; set; }
    public int Score { get; set; }
}

public class SetDecisionRequest
{
    public string Decision { get; set; } = string.Empty;
}