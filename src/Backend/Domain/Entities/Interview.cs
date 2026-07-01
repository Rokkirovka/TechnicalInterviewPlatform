using Domain.Enums;

namespace Domain.Entities;

public class Interview : BaseEntity
{
    public int ApplicationId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Comments { get; set; }
    public InterviewDecision InterviewDecision { get; set; } = InterviewDecision.Pending;

    public int? CreatedByUserId { get; set; }
    public int? DecidedByUserId { get; set; }

    public virtual Application Application { get; set; } = null!;
    public virtual User? CreatedByUser { get; set; }
    public virtual User? DecidedByUser { get; set; }
    
    public virtual ICollection<InterviewStage> InterviewStages { get; set; } = new List<InterviewStage>();
    public virtual ICollection<CompetencyScore> CompetencyScores { get; set; } = new List<CompetencyScore>();
}