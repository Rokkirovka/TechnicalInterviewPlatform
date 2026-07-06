using Application;
using Application.Dtos;
using Application.Interfaces;
using Infrastructure.Auth.Helpers;

namespace Infrastructure.Auth;

public class AuthService(
    ITokenService tokenService,
    IUserRepository userRepository, 
    PasswordHasher passwordHasher
    ) : IAuthService
{
    public async Task<LoginDto> LoginAsync(string login, string password, CancellationToken ct = default)
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

    public async Task LogoutAsync(int userId, CancellationToken ct = default)
    {
        await tokenService.RevokeTokenAsync(userId, ct);
    }
}
