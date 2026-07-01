namespace Domain.Entities;

public class Candidate : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string PreviousJob { get; set; } = string.Empty;

    public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}