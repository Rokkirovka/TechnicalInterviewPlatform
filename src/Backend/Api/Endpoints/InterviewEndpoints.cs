using System.Security.Claims;
using Api.Auth;
using Api.Helpers;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class InterviewEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapInterviewEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/interviews")
            .WithTags("Interviews");

        group.MapGet("/", async (
            [FromServices] IInterviewService interviewService,
            ClaimsPrincipal user,
            string? search,
            bool? showArchived = false) =>
        {
            if (showArchived is true && !user.IsInRole("admin") && !user.IsInRole("hr"))
                return Results.Forbid();
            var list = await interviewService.SearchAsync(search, showArchived ?? false);
            return Results.Ok(list);
        }).WithName("Search interviews")
        .WithSummary("Search interviews")
        .WithDescription("Find interviews by candidate name, may search among archived")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapGet("/{id}", async (int id, IInterviewService interviewService) =>
        {
            try
            {
                var interview = await interviewService.GetByIdAsync(id);
                return Results.Ok(interview);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Interview by id")
        .WithSummary("Get interview by id")
        .WithDescription("Find interview with given id")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.Authenticated);

        group.MapPost("/", async (
            [FromServices] IInterviewService interviewService,
            ClaimsPrincipal userClaims,
            CreateInterviewRequest request) =>
        {
            var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var id))
            {
                return Results.Unauthorized();
            }
            var result = await interviewService.CreateAsync(request, createdByUserId: id);
            return Results.Created($"/interviews/{result.Id}", result);
        }).WithName("Create interview")
        .WithSummary("Create new interview")
        .WithDescription("Create new interview with stages, matrix generated from vacancy")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPut("/{id}", async (
            [FromServices] IInterviewService interviewService,
            ClaimsPrincipal user,
            int id,
            UpdateInterviewRequest request) =>
        {
            try
            {
                var userId = user.GetUserId();
                var result = await interviewService.UpdateAsync(id, request, userId);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Update interview")
        .WithSummary("Update existing interview")
        .WithDescription("Update interview date, competency scores and add comment")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/{id}/mark-passed", async (
            [FromServices] IInterviewService interviewService,
            int id) =>
        {
            try
            {
                await interviewService.MarkPassedAsync(id);
                var result = await interviewService.GetByIdAsync(id);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Mark interview passed")
        .WithSummary("Mark interview as passed")
        .WithDescription("HR marks interview as passed")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/{id}/decision", async (
            [FromServices] IInterviewService interviewService,
            int id,
            SetDecisionRequest request) =>
        {
            try
            {
                await interviewService.SetDecisionAsync(id, request.Decision);
                var result = await interviewService.GetByIdAsync(id);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Set interview decision")
        .WithSummary("Set decision for interview")
        .WithDescription("Decision maker sets final decision (approved/rejected/to_next_stage)")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndApprover);

        group.MapPost("/{id}/archive", async (
            [FromServices] IInterviewService interviewService,
            ClaimsPrincipal userClaims,
            int id, [FromBody] ArchiveRequest request) =>
        {
            var claimId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var adminId))
                return Results.Unauthorized();

            try
            {
                await interviewService.ArchiveAsync(id, request.Reason, adminId);
                var result = await interviewService.GetByIdAsync(id);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Archive interview")
        .WithSummary("Archive interview")
        .WithDescription("Archive interview with reason")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/{id}/restore", async (
            [FromServices] IInterviewService interviewService,
            int id) =>
        {
            try
            {
                await interviewService.RestoreAsync(id);
                var result = await interviewService.GetByIdAsync(id);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("Restore interview")
        .WithSummary("Restore interview")
        .WithDescription("Restore interview from archive")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        return endpoints;
    }
}
