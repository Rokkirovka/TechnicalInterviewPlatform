using Domain.Enums;

namespace Application.Dtos;

public class CandidateDto : BaseDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string PreviousJob { get; set; } = string.Empty;
    public List<CandidateSkillDto> Skills { get; set; } = [];
    public CandidateStatus Status { get; set; }
}

public class CandidateSkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel Level { get; set; }
}

public class CreateCandidateRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string PreviousJob { get; set; } = string.Empty;
    public List<CreateCandidateSkillRequest> Skills { get; set; } = new();
}

public class CreateCandidateSkillRequest
{
    public int SkillId { get; set; }
    public ProficiencyLevel Level { get; set; }
}

public class UpdateCandidateRequest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string PreviousJob { get; set; } = string.Empty;
    public List<UpdateCandidateSkillRequest> Skills { get; set; } = new();
}

public class UpdateCandidateSkillRequest
{
    public int SkillId { get; set; }
    public ProficiencyLevel Level { get; set; }
}