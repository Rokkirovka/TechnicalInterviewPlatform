namespace Domain.Entities;

public class CompetencyScore
{
    public int Id { get; set; }
    public int InterviewId { get; set; }
    public int CompetencyId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Interview Interview { get; set; } = null!;
    public virtual Competency Competency { get; set; } = null!;
}