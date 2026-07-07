using Infrastructure.Auth.Entities;

namespace Infrastructure.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default);
    Task StoreAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task DeleteAllAsync(int userId, CancellationToken ct = default);
}
