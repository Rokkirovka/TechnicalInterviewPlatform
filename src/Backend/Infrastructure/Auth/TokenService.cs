using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Application;
using Domain;
using Domain.Entities;
using Infrastructure.Auth.Entities;
using Infrastructure.Auth.Helpers;
using Infrastructure.Auth.Options;
using Infrastructure.Data.Repositories;

namespace Infrastructure.Auth;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    JwtSettings jwtSettings
    ) : ITokenService
{
    public async Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = CreateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user, ct);
        
        return new LoginResult(accessToken, refreshToken.token, refreshToken.ExpiresAt);
    }
    
    public string CreateAccessToken(User user)
    {
        var claims = ClaimService.ConfigureUserClaims(user);
        return TokenGenerator.GenerateAccessToken(claims, jwtSettings);
    }

    public async Task<(string token, DateTime ExpiresAt)> CreateRefreshTokenAsync(User user, CancellationToken ct)
    {
        var token = TokenGenerator.GenerateRefreshToken();
        
        var tokenHash = HashToken(token);
        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            User = user,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays),
            IsUsed = false,
            IsRevoked = false
        };
        
        await refreshTokenRepository.StoreAsync(refreshToken, ct);
        return (token, refreshToken.ExpiresAt);
    }
    
    public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh or access token is required");
        }
        
        var hashedToken = HashToken(refreshToken);
        var storedRefreshToken = await refreshTokenRepository.GetByHashedTokenAsync(hashedToken, ct);
        if (storedRefreshToken is not { IsValid: true })
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = await userRepository.GetByIdAsync(storedRefreshToken.UserId, ct);
        if (user == null || user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        await refreshTokenRepository.MarkAsRevokedAsync(storedRefreshToken, ct);        
        
        return await GenerateTokensAsync(user, ct);
    }

    public async Task RevokeTokenAsync(int userId, CancellationToken ct)
    {
        var tokens = await refreshTokenRepository.GetValidUserTokensAsync(userId, ct);
        
        foreach (var token in tokens)
        {
            await refreshTokenRepository.MarkAsRevokedAsync(token, ct);
        }
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}