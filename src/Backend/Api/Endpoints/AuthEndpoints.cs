using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Api.Helpers;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth")
            .WithTags("Authentication")
            .AllowAnonymous();

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Log in")
            .WithDescription("Log in into account with company login and password")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", RefreshTokenAsync)
            .WithName("Refresh")
            .WithSummary("Refresh access token")
            .WithDescription("Refresh user access and refresh tokens via his refresh token")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        
        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("Logout into account")
            .WithDescription("Logout into account via removing access and refresh tokens")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        
        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest loginRequest, 
        IAuthService authService, 
        HttpContext context,
        CancellationToken ct
        )
    {
        try
        {
            var result = await authService.LoginAsync(loginRequest.Login, loginRequest.Password, ct);
            
            context.Response.SetTokensCookie(result.AccessToken, result.RefreshToken, result.ExpiresAt);
            
            return Results.Ok(new { message = "You successfully logged in." });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> RefreshTokenAsync(
        ITokenService tokenService, 
        HttpContext context,
        CancellationToken ct
        )
    {
        var refreshToken = context.Request.GetRefreshTokenCookie();
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Results.Unauthorized();
        }
        context.Response.ClearTokensCookie();

        try
        {
            var result = await tokenService.RefreshTokenAsync(refreshToken, ct);

            context.Response.SetTokensCookie(result.AccessToken, result.RefreshToken, result.ExpiresAt);

            return Results.Ok(new { message = "You successfully refresh user's tokens" });
        }
        catch (ValidationException)
        {
            return Results.BadRequest();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> LogoutAsync(
        ITokenService tokenService,
        HttpContext context,
        CancellationToken ct
        )
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Results.Unauthorized();
        }
        var userId = int.Parse(userIdClaim);
        
        await tokenService.RevokeTokenAsync(userId, ct);
        
        context.Response.ClearTokensCookie();

        return Results.NoContent();
    }
}