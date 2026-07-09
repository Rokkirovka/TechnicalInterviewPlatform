using System.Security.Claims;
using Api.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class VacanciesEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapVacanciesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/vacancies")
            .WithTags("Vacancies");

        group.MapGet("/{id}", async (int id, IVacancyService vacancyService) =>
        {
            var user = await vacancyService.GetByIdAsync(id);
            if (user != null)
            {
                return Results.Ok(user);
            }
            return Results.NotFound();
        }).WithName("Vacancy By Id")
        .WithSummary("Get vacancy by id")
        .WithDescription("Find vacancy in with given id")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapGet("/", async (
            [FromServices] IVacancyService vacancyService, 
            ClaimsPrincipal user,
            string? search,
            bool? showArchived = false) =>
        {
            if (showArchived is true && !user.IsInRole("admin") && !user.IsInRole("hr"))
            {
                return Results.Forbid();
            }
            var list = await vacancyService.SearchAsync(search, showArchived ?? false);
            return Results.Ok(list);
        }).WithName("Search vacancy")
        .WithSummary("Search vacancy")
        .WithDescription("Find vacancy with given name, may search among archived")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapPost("/", async (
            [FromServices] IVacancyService vacancyService,
            CreateVacancyRequest request) =>
        {
            var result = await vacancyService.CreateAsync(request);
            return Results.Ok(result);
        }).WithName("Create vacancy")
        .WithSummary("Create new vacancy")
        .WithDescription("Create new vacancy")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPut("/{id}", async (
            [FromServices] IVacancyService vacancyService,
            int id,
            UpdateVacancyRequest request) =>
        {
            var result = await vacancyService.UpdateAsync(id, request);
            return Results.Ok(result);
        }).WithName("Update vacancy")
        .WithSummary("Update existing vacancy")
        .WithDescription("Update existing vacancy")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/{id}/archive", async (
            [FromServices] IVacancyService vacancyService,
            ClaimsPrincipal userClaims,
            int id, [FromBody] ArchiveRequest request) =>
        {
            var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var adminId))
            {
                return Results.Unauthorized();
            }

            await vacancyService.ArchiveAsync(id, archivedByUserId: adminId, reason: request.Reason);
            var newUser = await vacancyService.GetByIdAsync(id);
            return Results.Ok(newUser);
        }).WithName("Archive vacancy")
        .WithSummary("Archive vacancy")
        .WithDescription("Archive vacancy")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);
        
        group.MapPost("/{id}/restore", async (
            [FromServices] IVacancyService vacancyService,
            ClaimsPrincipal userClaims,
            int id) =>
        {
            await vacancyService.RestoreAsync(id);
            return Results.Ok();
        }).WithName("Restore vacancy")
        .WithSummary("Restore vacancy")
        .WithDescription("Restore vacancy")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        return endpoints;
    }
}