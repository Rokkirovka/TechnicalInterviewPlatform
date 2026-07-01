namespace Domain.Entities;

public class Competency : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxScore { get; set; } = 5;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<CompetencyScore> CompetencyScores { get; set; } = new List<CompetencyScore>();
}