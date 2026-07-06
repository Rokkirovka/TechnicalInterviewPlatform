namespace Application.Dtos;

public record LoginDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
