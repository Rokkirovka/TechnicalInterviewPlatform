namespace Api.Helpers;

/// <summary>
/// 
/// </summary>
public static class TokensCookiesService
{
    private const string RefreshTokenCookieName = "refresh_token";
    private const string AccessTokenCookieName = "access_token";
    private const string CookiesPath = "/";
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="accessToken"></param>
    /// <param name="refreshToken"></param>
    /// <param name="expiresAt"></param>
    public static void SetTokensCookie(
        this HttpResponse response, 
        string accessToken, 
        string refreshToken, 
        DateTime expiresAt
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
        
        response.Cookies.Append(AccessTokenCookieName, accessToken, cookieOptions);
        response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    public static void ClearTokensCookie(this HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiesPath
        };
        
        response.Cookies.Delete(RefreshTokenCookieName, cookieOptions);
        response.Cookies.Delete(AccessTokenCookieName, cookieOptions);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string? GetRefreshTokenCookie(this HttpRequest request)
    {
        return request.Cookies[RefreshTokenCookieName];
    }
}
