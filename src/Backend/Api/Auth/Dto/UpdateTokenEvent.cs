namespace Api.Auth.Dto;

/// <summary>
/// 
/// </summary>
public record struct UpdateTokenEvent(string Token, DateTime ExpiresAt);