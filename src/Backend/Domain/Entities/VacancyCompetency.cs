namespace Domain.Entities;

public class VacancyCompetency
{
    public int VacancyId { get; set; }
    public int CompetencyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual Competency Competency { get; set; } = null!;
}