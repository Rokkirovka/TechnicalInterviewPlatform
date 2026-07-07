using Application.Dtos;

namespace Application;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string login, string password, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
}