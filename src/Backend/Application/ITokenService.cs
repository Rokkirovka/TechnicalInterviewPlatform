using Domain.Entities;

namespace Application;

public interface ITokenService
{
    Task<LoginResult> GenerateTokensAsync(User user, CancellationToken ct);
    Task<LoginResult> UpdateTokenAsync(string refreshToken, CancellationToken ct);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task RevokeAllTokensAsync(int userId, CancellationToken ct);
}