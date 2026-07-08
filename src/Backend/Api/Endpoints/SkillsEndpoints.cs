using Api.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class SkillsEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapSkillsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/skills")
            .WithTags("Skills");
            
        group.MapGet("/", async (
            [FromServices] ISkillService skillService) =>
        {
            var list = await skillService.GetAllAsync();
            return Results.Ok(list);
        }).WithName("Search skills")
        .WithSummary("Search skills")
        .WithDescription("Find skills")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/", async (
            [FromServices] ISkillService skillService,
            CreateSkillRequest request) =>
            {
                await skillService.CreateAsync(request);
                return Results.Created();
            }).WithName("Create skill")
        .WithSummary("Create new skill")
        .WithDescription("Create new skill")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);
        
        group.MapPut("/{id}", async (
            [FromServices] ISkillService skillService,
            int id,
            UpdateSkillRequest request) =>
        {
            var result = await skillService.UpdateAsync(id, request);
            return Results.Ok(result);
        }).WithName("Update skill")
        .WithSummary("Update skill")
        .WithDescription("Update skill")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        return endpoints;
    }
}