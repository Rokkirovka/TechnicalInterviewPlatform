using System.Security.Claims;
using Api.Auth.Options;
using Api.Auth.Services;
using Api.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Log in")
            .WithDescription("Log in into account with company login and password")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

        group.MapPost("/refresh", RefreshTokenAsync)
            .WithName("Refresh")
            .WithSummary("Refresh access token")
            .WithDescription("Refresh user access and refresh tokens via his refresh token")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        group.MapGet("/me", MeAsync)
            .WithName("Me")
            .WithSummary("Send user info")
            .WithDescription("Send information about current user")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("Logout from account")
            .WithDescription("Logout from account via removing access and refresh tokens")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest loginRequest,
        IAuthService authService,
        HttpContext context,
        IOptions<JwtSettings> jwtSettings,
        CancellationToken ct
        )
    {
        var result = await authService.LoginAsync(loginRequest.Login, loginRequest.Password, ct);

        if (!result.User.IsActive)
        {
            return Results.Forbid();
        }
      
        context.Response.SetTokensCookie(result.NewAccessToken, result.NewRefreshToken, jwtSettings.Value);
        
        return Results.Ok(new
        {
            token = result.NewAccessToken.Token,
            user = result.User
        });
    }

    private static async Task<IResult> MeAsync(
        ClaimsPrincipal userClaims,
        IUserService userService
    )
    {
        var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var id))
        {
            return Results.Unauthorized();
        }
        var user = await userService.GetByIdAsync(id);
        return Results.Ok(user);
    }

    private static async Task<IResult> RefreshTokenAsync(
        ITokenService tokenService,
        HttpContext context,
        IOptions<JwtSettings> jwtSettings,
        CancellationToken ct
        )
    {
        var refreshToken = context.Request.GetRefreshTokenCookie(jwtSettings.Value);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Results.Unauthorized();
        }
        context.Response.ClearTokensCookie(jwtSettings.Value);

        var result = await tokenService.UpdateTokenAsync(refreshToken, ct);

        context.Response.SetTokensCookie(result.NewAccessToken, result.NewRefreshToken, jwtSettings.Value);

        return Results.Ok(new { token = result.NewAccessToken.Token });
    }

    private static async Task<IResult> LogoutAsync(
        ClaimsPrincipal userClaims,
        IAuthService authService,
        HttpContext context,
        IOptions<JwtSettings> jwtSettings,
        CancellationToken ct
        )
    {
        var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var userId))
        {
            return Results.Unauthorized();
        }

        var refreshToken = context.Request.GetRefreshTokenCookie(jwtSettings.Value);
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await authService.LogoutAsync(refreshToken, userId, ct);
        }

        context.Response.ClearTokensCookie(jwtSettings.Value);

        return Results.NoContent();
    }
}