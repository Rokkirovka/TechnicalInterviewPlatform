namespace Domain.Entities;

public class Vacancy : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public virtual ICollection<VacancyCompetency> VacancyCompetencies { get; set; } = new List<VacancyCompetency>();
}