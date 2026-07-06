using Domain.Entities;

namespace Application;

public interface ITokenService
{
    Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct);
    Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task RevokeTokenAsync(int userId, CancellationToken ct);
}