namespace Domain.Entities;

public class Competency : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<VacancyCompetency> VacancyCompetencies { get; set; } = new List<VacancyCompetency>();
    public virtual ICollection<CompetencyScore> CompetencyScores { get; set; } = new List<CompetencyScore>();
}