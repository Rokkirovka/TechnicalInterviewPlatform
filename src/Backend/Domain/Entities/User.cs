namespace Domain.Entities;

public class User : BaseEntity
{
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName} {MiddleName}";
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Role> Roles { get; set; } = [];
}