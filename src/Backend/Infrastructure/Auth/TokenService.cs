using System.ComponentModel.DataAnnotations;
using Application;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Auth.Entities;
using Infrastructure.Auth.Helpers;
using Infrastructure.Auth.Options;
using Infrastructure.Repositories;

namespace Infrastructure.Auth;

public class TokenService(
    TokenHasher tokenHasher,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    JwtSettings jwtSettings
    ) : ITokenService
{
    public async Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = TokenGenerator.GenerateAccessToken(user, jwtSettings);
        var refreshToken = await CreateRefreshTokenAsync(user);
        
        return new LoginResult(accessToken, refreshToken.token, refreshToken.ExpiresAt);
    }
    
    public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh or access token is required");
        }
        
        var hashedToken = tokenHasher.HashToken(refreshToken);
        var storedRefreshToken = await refreshTokenRepository.GetByHashedTokenAsync(hashedToken, ct);
        if (storedRefreshToken is not { IsValid: true })
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = await userRepository.GetByIdAsync(storedRefreshToken.UserId);
        if (user is not { IsActive: true })
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
    
    private async Task<(string token, DateTime ExpiresAt)> CreateRefreshTokenAsync(User user)
    {
        var token = TokenGenerator.GenerateRefreshToken();
        
        var tokenHash = tokenHasher.HashToken(token);
        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            User = user,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };
        
        await refreshTokenRepository.AddAsync(refreshToken);
        return (token, refreshToken.ExpiresAt);
    }
}