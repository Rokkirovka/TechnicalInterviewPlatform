using Domain.Enums;

namespace Domain.Entities;

public class CandidateSkill
{
    public int CandidateId { get; set; }
    public int SkillId { get; set; }
    public ProficiencyLevel Level { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public virtual Candidate Candidate { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
}