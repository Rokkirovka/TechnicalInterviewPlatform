using Domain.Entities;

namespace Application.Dtos;

public class UserDto : BaseDto
{
    public string Login { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool Archived { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class CreateUserRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = new();
}

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<Role> Roles { get; set; } = new();
}