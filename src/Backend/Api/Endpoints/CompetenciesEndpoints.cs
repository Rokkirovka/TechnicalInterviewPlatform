using Api.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// 
/// </summary>
public static class CompetenciesEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapCompetenciesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/competencies")
            .WithTags("Competencies");
            
        group.MapGet("/", async (
            [FromServices] ICompetencyService competencyService) =>
        {
            var list = await competencyService.GetAllAsync();
            return Results.Ok(list);
        }).WithName("Search competencies")
        .WithSummary("Search competencies")
        .WithDescription("Find competencies")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapPost("/", async (
            [FromServices] ICompetencyService competencyService,
            CreateCompetencyRequest request) =>
        {
            var result = await competencyService.CreateAsync(request);
            return Results.Ok(result);
        }).WithName("Create competency")
        .WithSummary("Create new competency")
        .WithDescription("Create new competency")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);
        
        group.MapPut("/", async (
            [FromServices] ICompetencyService competencyService,
            UpdateCompetencyRequest request) =>
        {
            var result = await competencyService.UpdateAsync(request);
            return Results.Ok(result);
        }).WithName("Update competency")
        .WithSummary("Update competency")
        .WithDescription("Update competency")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        return endpoints;
    }
}