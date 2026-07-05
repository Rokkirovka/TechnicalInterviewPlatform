namespace Domain.Entities;

public class CompetencyScore : BaseEntity
{
    public int InterviewId { get; set; }
    public int CompetencyId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }

    public virtual Interview Interview { get; set; } = null!;
    public virtual Competency Competency { get; set; } = null!;
}