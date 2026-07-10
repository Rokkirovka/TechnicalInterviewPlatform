namespace Domain.Entities;

public class InterviewStage : BaseEntity
{
    public int InterviewId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Description { get; set; }
    public int OrderNumber { get; set; }

    public virtual Interview Interview { get; set; } = null!;
}