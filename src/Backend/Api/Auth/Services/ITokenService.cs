using Api.Auth.Dto;
using Domain.Entities;

namespace Api.Auth.Services;

/// <summary>
/// 
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<LoginResult> UpdateTokenAsync(string refreshToken, CancellationToken ct);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RevokeAllTokensAsync(int userId, CancellationToken ct);
}