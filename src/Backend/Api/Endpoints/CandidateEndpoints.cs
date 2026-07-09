using System.Security.Claims;
using Api.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class CandidateEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapCandidateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/candidates")
            .WithTags("Candidates");

        group.MapGet("/{id}", async (int id, ICandidateService candidateService) =>
        {
            var user = await candidateService.GetByIdAsync(id);
            if (user != null)
            {
                return Results.Ok(user);
            }
            return Results.NotFound();
        }).WithName("CandidateById")
        .WithSummary("Get candidate by id")
        .WithDescription("Find candidate in with given id")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapGet("/", async (
            [FromServices] ICandidateService candidateService,
            ClaimsPrincipal user,
            string? search,
            bool? showArchived = false) =>
        {
            if (showArchived is true && !user.IsInRole("admin") && !user.IsInRole("hr"))
            {
                return Results.Forbid();
            }
            var list = await candidateService.SearchAsync(search, showArchived ?? false);
            return Results.Ok(list);
        }).WithName("Search candidate")
        .WithSummary("Search candidate")
        .WithDescription("Find candidate with given name, may search among archived")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapGet("/names", async (ICandidateService candidateService) =>
        {
            var users = await candidateService.GetNamesAsync();
            return Results.Ok(users.Select(u => new { id=u.Id, fullName=u.FullName }));
        }).WithName("CandidateNames")
        .WithSummary("Get some candidates names")
        .WithDescription("Get some candidates names")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapGet("/{id}/interviews", async (int id, IInterviewService interviewService) =>
        {
            var list = await interviewService.GetByCandidateIdAsync(id);
            return Results.Ok(list);
        }).WithName("Interviews for candidate")
        .WithSummary("Get interviews for candidate")
        .WithDescription("Get interviews for candidate")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapPost("/", async (
            [FromServices] ICandidateService candidateService,
            CreateCandidateRequest request) =>
        {
            var result = await candidateService.CreateAsync(request);
            return Results.Ok(result);
        }).WithName("Create candidate")
        .WithSummary("Create new candidate")
        .WithDescription("Create new candidate")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPut("/{id}", async (
            [FromServices] ICandidateService candidateService,
            int id,
            UpdateCandidateRequest request) =>
        {
            var result = await candidateService.UpdateAsync(id, request);
            return Results.Ok(result);
        }).WithName("Update candidate")
        .WithSummary("Update existing candidate")
        .WithDescription("Update existing candidate")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/{id}/archive", async (
            [FromServices] ICandidateService candidateService,
            ClaimsPrincipal userClaims,
            int id, [FromBody] ArchiveRequest request) =>
        {
            var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var adminId))
            {
                return Results.Unauthorized();
            }
            // TODO добавить проверку, вдруг пользователь уже заархивирован

            await candidateService.ArchiveAsync(id, archivedByUserId: adminId, reason: request.Reason);
            var newUser = await candidateService.GetByIdAsync(id);
            return Results.Ok(newUser);
        }).WithName("Archive candidate")
        .WithSummary("Archive candidate")
        .WithDescription("Archive candidate")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);
        
        group.MapPost("/{id}/restore", async (
            [FromServices] ICandidateService candidateService,
            ClaimsPrincipal userClaims,
            int id) =>
        {
            await candidateService.RestoreAsync(id);
            var newUser = await candidateService.GetByIdAsync(id);
            return Results.Ok(newUser);
        }).WithName("Restore candidate")
        .WithSummary("Restore candidate")
        .WithDescription("Restore candidate")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        return endpoints;
    }
}