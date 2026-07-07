using System.ComponentModel.DataAnnotations;
using Api.Auth.Helpers;
using Api.Auth.Options;
using Application;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Auth;
using Microsoft.Extensions.Options;

namespace Api.Auth;

/// <summary>
/// 
/// </summary>
/// <param name="refreshTokenRepository"></param>
/// <param name="userRepository"></param>
/// <param name="jwtSettings"></param>
public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IOptions<JwtSettings> jwtSettings
    ) : ITokenService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = TokenGenerator.GenerateAccessToken(user, jwtSettings.Value);
        var (token, ExpiresAt) = await CreateRefreshTokenAsync(user, ct);

        return new LoginResult(accessToken, token, ExpiresAt);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<LoginResult> UpdateTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh or access token is required");
        }

        var hashedToken = TokenHasher.HashToken(refreshToken);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException("Refresh token is required");
        }

        var hashedToken = TokenHasher.HashToken(refreshToken);
        var token = await refreshTokenRepository.GetByHashedTokenAsync(hashedToken, ct);

        if (token == null)
        {
            return;
        }

        await refreshTokenRepository.DeleteAsync(token, ct);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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

        var tokenHash = TokenHasher.HashToken(token);
        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays),
        };

        await refreshTokenRepository.StoreAsync(refreshToken, ct);
        return (token, refreshToken.ExpiresAt);
    }
}