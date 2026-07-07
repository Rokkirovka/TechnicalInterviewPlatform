using Domain.Entities;

namespace Infrastructure.Auth.Entities;

public class RefreshToken
{
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsValid => DateTime.UtcNow < ExpiresAt;
}
