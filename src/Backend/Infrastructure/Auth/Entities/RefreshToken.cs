using Domain.Entities;

namespace Infrastructure.Auth.Entities;

public class RefreshToken : BaseEntity
{
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsValid => !IsRevoked && DateTime.UtcNow < ExpiresAt;
}
