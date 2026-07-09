using Api.Auth.Dto;
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
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
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
            logger.LogWarning("Неудачная попытка входа. Пользователь не найден.");
            
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        if (!passwordHasher.IsPasswordVerified(user, password))
        {
            logger.LogWarning("Неудачная попытка входа для пользователя {UserId}. Неверный пароль.", user.Id);
            
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        if (!user.IsActive)
        {
            logger.LogInformation("Пользователь {UserId} неактивен", user.Id);
            
            throw new UnauthorizedAccessException("User not found or inactive");
        }
        
        var result = await tokenService.GenerateTokensAsync(user, ct);
        
        logger.LogInformation("Пользователь {UserId} успешно аутентифицирован.", user.Id);
        
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task LogoutAsync(string refreshToken, int userId, CancellationToken ct = default)
    {
        await tokenService.RevokeRefreshTokenAsync(refreshToken, ct);
        
        logger.LogInformation("Пользователь {UserId} успешно вышел.", userId);
    }
}
