using Domain.Entities;

namespace Infrastructure.Auth.Helpers;

public class PasswordHasher
{
    private const int WorkFactor = 12;

    public string HashPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public bool IsPasswordVerified(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

}