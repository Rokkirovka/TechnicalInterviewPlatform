using System.Security.Claims;

namespace Api.Helpers;

/// <summary>
/// 
/// </summary>
public static class ClaimsUserAccessor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(value, out var id) ? 
             id : throw new UnauthorizedAccessException();
    }
}
