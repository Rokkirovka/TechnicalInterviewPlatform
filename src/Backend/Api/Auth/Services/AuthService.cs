using Api.Auth.Helpers;
using Application;
using Application.Dtos;
using Application.Interfaces;

namespace Api.Auth.Services;

/// <summary>
/// 
/// </summary>
/// <param name="tokenService"></param>
/// <param name="userRepository"></param>
/// <param name="passwordHasher"></param>
public class AuthService(
    ITokenService tokenService,
    IUserRepository userRepository, 
    IPasswordHasher passwordHasher
    ) : IAuthService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var user = await userRepository.GetByLoginAsync(login);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        if (!passwordHasher.IsPasswordVerified(user, password))
        {
            throw new UnauthorizedAccessException("Invalid login or password");
        }
        
        // ?: можно ввести у пользователя поле LastLogin и обновлять его тут
        
        return await tokenService.GenerateTokensAsync(user, ct);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await tokenService.RevokeRefreshTokenAsync(refreshToken, ct);
    }
}
