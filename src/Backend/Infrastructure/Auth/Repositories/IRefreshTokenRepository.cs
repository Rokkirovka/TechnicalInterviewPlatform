using Application.Interfaces;
using Infrastructure.Auth.Entities;

namespace Infrastructure.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default);
    // Task StoreAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task MarkAsRevokedAsync(RefreshToken refreshToken, CancellationToken ct = default);
}
