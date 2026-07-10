using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Api.Auth.Helpers;

/// <summary>
/// Хэширует пароли
/// </summary>
public class PasswordHasher : Application.Interfaces.IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool IsPasswordVerified(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}