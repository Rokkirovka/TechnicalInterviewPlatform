using System.ComponentModel.DataAnnotations;
using Application;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Auth.Entities;
using Infrastructure.Auth.Helpers;
using Infrastructure.Auth.Options;
using Infrastructure.Auth.Repositories;

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
        var (token, ExpiresAt) = await CreateRefreshTokenAsync(user, ct);

        return new LoginResult(accessToken, token, ExpiresAt);
    }

    public async Task<LoginResult> UpdateTokenAsync(string refreshToken, CancellationToken ct = default)
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

        await refreshTokenRepository.DeleteAsync(storedRefreshToken, ct);

        return await GenerateTokensAsync(user, ct);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh token is required");
        }

        var hashedToken = tokenHasher.HashToken(refreshToken);
        var token = await refreshTokenRepository.GetByHashedTokenAsync(hashedToken, ct);

        if (token == null)
        {
            return;
        }

        await refreshTokenRepository.DeleteAsync(token, ct);
    }

    public async Task RevokeAllTokensAsync(int userId, CancellationToken ct)
    {
        var tokens = await refreshTokenRepository.GetValidUserTokensAsync(userId, ct);

        foreach (var token in tokens)
        {
            await refreshTokenRepository.DeleteAsync(token, ct);
        }
    }

    private async Task<(string token, DateTime ExpiresAt)> CreateRefreshTokenAsync(User user, CancellationToken ct)
    {
        var token = TokenGenerator.GenerateRefreshToken();

        var tokenHash = tokenHasher.HashToken(token);
        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays),
        };

        await refreshTokenRepository.StoreAsync(refreshToken, ct);
        return (token, refreshToken.ExpiresAt);
    }
}