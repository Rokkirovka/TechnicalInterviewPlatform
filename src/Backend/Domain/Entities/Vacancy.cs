namespace Domain.Entities;

public class Vacancy : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}