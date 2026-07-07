namespace Application.Dtos;

public record LoginResult(string AccessToken, string RefreshToken, DateTime ExpiresAt);
