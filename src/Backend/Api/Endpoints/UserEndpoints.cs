using System.Security.Claims;
using Api.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users")
            .WithTags("User");

        group.MapGet("/{id}", async (int id, IUserService userService) =>
        {
            var user = await userService.GetByIdAsync(id);
            if (user != null)
            {
                return Results.Ok(user);
            }
            return Results.NotFound();
        }).WithName("UserById")
        .WithSummary("Get user by id")
        .WithDescription("Find user in database with given id")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapGet("/", async (
            [FromServices] IUserService userService,
            string? search,
            bool? showArchived = false) =>
        {
            var list = (showArchived ?? false)
                ? await userService.GetAllDeletedAsync()
                : await userService.GetAllAliveAsync();

            return Results.Ok(list);
        }).WithName("Search")
        .WithSummary("Search user")
        .WithDescription("Find user with given name, may search among archived")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapPost("/", async (
            [FromServices] IUserService userService,
            CreateUserRequest request) =>
        {
            // TODO properly check for duplicate logins
            var result = await userService.CreateAsync(request);
            return Results.Ok(result);
        }).WithName("Create")
        .WithSummary("Create new user")
        .WithDescription("Create new user")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapPut("/", async (
            [FromServices] IUserService userService,
            UpdateUserRequest request) =>
        {
            var result = await userService.UpdateAsync(request);
            return Results.Ok(result);
        }).WithName("Update")
        .WithSummary("Update existing user")
        .WithDescription("Update existing user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapPost("/{id}/archive", async (
            [FromServices] IUserService userService,
            ClaimsPrincipal userClaims,
            int id, [FromBody] string reason) =>
        {
            var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var adminId))
            {
                return Results.Unauthorized();
            }
            // TODO добавить проверку, вдруг пользователь уже заархивирован

            await userService.DeleteAsync(id, deletedByUserId: adminId, reason: reason);
            var newUser = await userService.GetByIdIncludingDeletedAsync(id);
            return Results.Ok(newUser);
        }).WithName("Archive")
        .WithSummary("Archive user")
        .WithDescription("Archive user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);
        
        group.MapPost("/{id}/restore", async (
            [FromServices] IUserService userService,
            ClaimsPrincipal userClaims,
            int id) =>
        {
            await userService.RestoreAsync(id);
            return Results.Ok();
        }).WithName("Restore")
        .WithSummary("Restore")
        .WithDescription("Restore")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Admin);

        return endpoints;
    }
}