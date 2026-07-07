using Api.Auth.Options;

namespace Api.Helpers;

/// <summary>
/// 
/// </summary>
public static class TokensCookiesService
{
    private const string CookiesPath = "/";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="accessToken"></param>
    /// <param name="refreshToken"></param>
    /// <param name="expiresAt"></param>
    /// <param name="jwtSettings"></param>
    public static void SetTokensCookie(
        this HttpResponse response, 
        string accessToken, 
        string refreshToken, 
        DateTime expiresAt,
        JwtSettings jwtSettings
        )
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiesPath,
            Expires = expiresAt
        };
        
        response.Cookies.Append(jwtSettings.AccessTokenCookieName, accessToken, cookieOptions);
        response.Cookies.Append(jwtSettings.RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="jwtSettings"></param>
    public static void ClearTokensCookie(this HttpResponse response, JwtSettings jwtSettings)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiesPath
        };
        
        response.Cookies.Delete(jwtSettings.AccessTokenCookieName, cookieOptions);
        response.Cookies.Delete(jwtSettings.RefreshTokenCookieName, cookieOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="jwtSettings"></param>
    /// <returns></returns>
    public static string? GetRefreshTokenCookie(this HttpRequest request, JwtSettings jwtSettings)
    {
        return request.Cookies[jwtSettings.RefreshTokenCookieName];
    }
}
