namespace Application;

public record LoginResult(string AccessToken, string RefreshToken, DateTime ExpiresAt);
