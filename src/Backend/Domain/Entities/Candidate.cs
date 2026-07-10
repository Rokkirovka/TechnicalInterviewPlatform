namespace Domain.Entities;

public class Candidate : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName} {MiddleName}";
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string PreviousJob { get; set; } = string.Empty;

    public string Status => Interviews switch
    {
        { Count: 0 } => "new",
        _ when Interviews.Any(i => i.Status == Enums.InterviewStatus.Approved) => "accepted",
        _ => "in_progress"
    };

    public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = [];
    public virtual ICollection<Interview> Interviews { get; set; } = [];
}