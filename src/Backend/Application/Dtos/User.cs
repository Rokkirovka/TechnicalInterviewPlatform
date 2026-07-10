namespace Application.Dtos;

public class UserDto : BaseDto
{
    public string Login { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
    public bool Archived { get; set; }
}

public class CreateUserRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    /// <example>admin | hr | approver</example>
    public List<string> Roles { get; set; } = [];
}

public class UpdateUserRequest
{
    public string Login { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    /// <example>admin | hr | approver</example>
    public List<string> Roles { get; set; } = [];
}