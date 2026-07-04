using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Auth.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth.Helpers;

public class TokenGenerator
{
    public static string GenerateAccessToken(IEnumerable<Claim> claims, JwtSettings settings)
    {
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
    
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}