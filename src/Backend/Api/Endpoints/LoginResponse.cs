using Infrastructure.Auth.Entities;

namespace Api.Endpoints;

/// <summary>
/// Response model for user authentication (maybe not for use)
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// user's access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// user's refresh token
    /// </summary>
    public RefreshToken RefreshToken { get; set; } = new();
}
