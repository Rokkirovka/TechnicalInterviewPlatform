using Api.Auth.Dto;

namespace Api.Auth.Services;

/// <summary>
/// 
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<LoginResult> LoginAsync(string login, string password, CancellationToken ct = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
}