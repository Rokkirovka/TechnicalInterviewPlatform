using Infrastructure.Auth.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == refreshToken, ct);
    }

    public async Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId && DateTime.UtcNow < rt.ExpiresAt)
            .ToListAsync(ct);
    }

    public async Task StoreAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        context.Remove(refreshToken);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task DeleteAllAsync(int userId, CancellationToken ct = default)
    {
        await context.RefreshTokens.Where(t => t.UserId == userId)
            .ExecuteDeleteAsync(ct);
    }
}