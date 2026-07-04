using Infrastructure.Auth.Entities;

namespace Infrastructure.Data.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default);
    Task StoreAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task MarkAsUsedAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task MarkAsRevokedAsync(RefreshToken refreshToken, CancellationToken ct = default);
}