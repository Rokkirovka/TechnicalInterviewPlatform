using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Auth.Helpers;

public class PasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool IsPasswordVerified(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }

}