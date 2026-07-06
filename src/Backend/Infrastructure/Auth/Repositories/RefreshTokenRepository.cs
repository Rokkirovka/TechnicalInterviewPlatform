using Infrastructure.Auth.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) 
    : BaseRepository<RefreshToken>(context), 
        IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshToken?> GetByHashedTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == refreshToken, ct);
    }

    public async Task<ICollection<RefreshToken>> GetValidUserTokensAsync(int userId, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && DateTime.UtcNow < rt.ExpiresAt)
            .ToListAsync(ct);
    }
    
    public async Task MarkAsRevokedAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }
}