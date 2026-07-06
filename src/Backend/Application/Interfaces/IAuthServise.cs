using Application.Dtos;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginDto> LoginAsync(string login, string password, CancellationToken ct = default);
    Task LogoutAsync(int userId, CancellationToken ct = default);
}