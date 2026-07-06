namespace Application.Dtos;

public class VacancyDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<VacancyCompetencyDto> Competencies { get; set; } = new();
}

public class VacancyCompetencyDto
{
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
}

public class CreateVacancyRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<int> CompetencyIds { get; set; } = new();
}

public class UpdateVacancyRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<int> CompetencyIds { get; set; } = new();
}