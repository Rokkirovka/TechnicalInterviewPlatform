using Api.Auth;
using Api.Auth.Options;
using Api.Helpers;
using Application;
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
            .AllowAnonymous();
        
        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("Logout into account")
            .WithDescription("Logout into account via removing access and refresh tokens")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        group.MapGet("/admin-only", () => "This endpoint is Admin only")
            .RequireAuthorization(RoleBasedPolicies.Admin);
        
        group.MapGet("/hr", () => "This endpoint is for hr")
            .RequireAuthorization(RoleBasedPolicies.Hr);
        
        group.MapGet("/resolver", () => "This endpoint is for desition maker")
            .RequireAuthorization(RoleBasedPolicies.DecisionMaker);
        
        group.MapGet("/admin-hr", () => "This endpoint is for hr and admin")
            .RequireAuthorization(RoleBasedPolicies.AdminAndHr);
        
        group.MapGet("/admin-resolver", () => "This is for admin and decision maker")
            .RequireAuthorization(RoleBasedPolicies.AdminAndDecisionMaker);
        
        group.MapGet("/for-all", () => "This endpoint is for all")
            .RequireAuthorization(RoleBasedPolicies.Authenticated);
        
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

        context.Response.SetTokensCookie(result.AccessToken, result.RefreshToken, result.ExpiresAt, jwtSettings.Value);

        return Results.Ok(new { message = "You successfully logged in." });
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

        context.Response.SetTokensCookie(result.AccessToken, result.RefreshToken, result.ExpiresAt, jwtSettings.Value);

        return Results.Ok(new { message = "You successfully refresh user's tokens" });
    }

    private static async Task<IResult> LogoutAsync(
        IAuthService authService,
        HttpContext context,
        IOptions<JwtSettings> jwtSettings,
        CancellationToken ct
        )
    {
        var refreshToken = context.Request.GetRefreshTokenCookie(jwtSettings.Value);
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await authService.LogoutAsync(refreshToken, ct);
        }
        
        context.Response.ClearTokensCookie(jwtSettings.Value);

        return Results.NoContent();
    }
}