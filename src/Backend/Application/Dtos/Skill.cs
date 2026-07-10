namespace Application.Dtos;

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateSkillRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateSkillRequest
{
    public string Name { get; set; } = string.Empty;
}