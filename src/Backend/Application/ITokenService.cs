using Application.Dtos;
using Domain.Entities;

namespace Application;

public interface ITokenService
{
    Task<LoginDto> GenerateTokensAsync(User user, CancellationToken ct);
    Task<LoginDto> RefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task RevokeTokenAsync(int userId, CancellationToken ct);
}