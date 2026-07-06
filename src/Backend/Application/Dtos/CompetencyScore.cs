namespace Application.Dtos;

public class CompetencyScoreDto : BaseDto
{
    public int InterviewId { get; set; }
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string? Comment { get; set; }
}

public class CreateCompetencyScoreRequest
{
    public int InterviewId { get; set; }
    public int CompetencyId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}

public class UpdateCompetencyScoreRequest
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}