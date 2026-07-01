using Domain.Enums;

namespace Domain.Entities;

public class Application : BaseEntity
{
    public int CandidateId { get; set; }
    public int VacancyId { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Unscheduled;

    public virtual Candidate Candidate { get; set; } = null!;
    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual Interview? Interview { get; set; }
}