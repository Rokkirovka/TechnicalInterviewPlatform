namespace Application.Dtos;

public class InterviewStageDto : BaseDto
{
    public int InterviewId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
    public int OrderNumber { get; set; }
}

public class CreateInterviewStageRequest
{
    public int InterviewId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
    public int OrderNumber { get; set; }
}

public class UpdateInterviewStageRequest
{
    public int Id { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
    public int OrderNumber { get; set; }
}