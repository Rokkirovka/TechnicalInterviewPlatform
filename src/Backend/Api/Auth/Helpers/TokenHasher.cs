using System.Security.Cryptography;
using System.Text;

namespace Api.Auth.Helpers;

/// <summary>
/// 
/// </summary>
public static class TokenHasher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}