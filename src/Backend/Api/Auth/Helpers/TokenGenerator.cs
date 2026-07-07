using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Auth.Options;
using Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Api.Auth.Helpers;

/// <summary>
/// 
/// </summary>
public class TokenGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static string GenerateAccessToken(User user, JwtSettings settings)
    {
        var claims = ClaimService.ConfigureUserClaims(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}