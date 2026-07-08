using Domain.Enums;

namespace Application.Dtos;

public class CandidateNameDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class CandidateDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<CandidateSkillDto> Skills { get; set; } = new();
}

public class CandidateSkillDto
{
    public int Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel Level { get; set; }
}

public class CreateCandidateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public List<CreateCandidateSkillRequest> Skills { get; set; } = new();
}

public class CreateCandidateSkillRequest
{
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel Level { get; set; }
}

public class UpdateCandidateRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public List<UpdateCandidateSkillRequest> Skills { get; set; } = new();
}

public class UpdateCandidateSkillRequest
{
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel Level { get; set; }
}