using Infrastructure.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id, ct);
    }

    public async Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == refreshToken, ct);
    }

    public async Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.IsValid)
            .ToListAsync(ct);
    }

    public async Task StoreAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task MarkAsUsedAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.IsUsed = true;
        refreshToken.UsedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task MarkAsRevokedAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }
}