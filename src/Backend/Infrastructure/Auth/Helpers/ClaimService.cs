using System.Security.Claims;
using Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Auth.Helpers;

public class ClaimService
{
    public static List<Claim> ConfigureUserClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Login),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        
        return claims;
    }
}