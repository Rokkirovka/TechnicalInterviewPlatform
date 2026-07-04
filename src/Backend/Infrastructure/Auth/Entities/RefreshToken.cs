using Domain.Entities;

namespace Infrastructure.Auth.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsValid => !IsUsed && !IsRevoked && DateTime.UtcNow < ExpiresAt;
}
