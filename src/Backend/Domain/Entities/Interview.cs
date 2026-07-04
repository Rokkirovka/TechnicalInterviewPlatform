using Domain.Enums;

namespace Domain.Entities;

public class Interview : BaseEntity
{
    public int CandidateId { get; set; }
    public int VacancyId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public InterviewStatus Status { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? DecidedByUserId { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;
    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? AssignedToUser { get; set; }
    public virtual User? DecidedByUser { get; set; }
    
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<InterviewStage> InterviewStages { get; set; } = new List<InterviewStage>();
    public virtual ICollection<CompetencyScore> CompetencyScores { get; set; } = new List<CompetencyScore>();
}