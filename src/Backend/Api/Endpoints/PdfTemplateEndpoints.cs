using Api.Auth;
using Api.Helpers;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// Endpoints for PDF templates.
/// </summary>
public static class PdfTemplateEndpoints
{
    /// <summary>
    /// PDF
    /// </summary>
    public static IEndpointRouteBuilder MapPdfTemplateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/pdf-templates")
            .WithTags("PDF templates")
            .RequireAuthorization();

        group.MapPost("/", UploadAsync)
            .WithName("UploadPdfTemplate")
            .WithSummary("Upload fillable PDF template")
            .RequireAuthorization(RoleBasedPolicies.Admin)
            .DisableAntiforgery();

        group.MapGet("/", GetAvailableAsync)
            .WithName("GetPdfTemplates")
            .WithSummary("Get PDF templates available for generation")
            .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapGet("/candidate-fields", GetCandidateFieldsAsync)
            .WithName("GetCandidatePdfFields")
            .WithSummary("Get candidate fields available for PDF mapping")
            .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapPut("/{templateId:int}/mappings", SaveMappingsAsync)
            .WithName("SavePdfTemplateMappings")
            .WithSummary("Save candidate-to-PDF field mappings")
            .RequireAuthorization(RoleBasedPolicies.Admin);

        group.MapGet("/{templateId:int}/candidates/{candidateId:int}", GenerateAsync)
            .WithName("GenerateCandidatePdf")
            .WithSummary("Generate filled candidate PDF")
            .RequireAuthorization(RoleBasedPolicies.AdminAndHr);

        group.MapDelete("/{templateId:int}", DeleteAsync)
            .WithName("DeletePdfTemplate")
            .WithSummary("Delete PDF template")
            .RequireAuthorization(RoleBasedPolicies.Admin);

        return endpoints;
    }

    private static async Task<IResult> UploadAsync(
        [FromForm] UploadPdfTemplateRequest request,
        IPdfTemplateService service,
        HttpContext context,
        CancellationToken ct)
    {
        var file = request.File;
        if (file.Length == 0)
            return Results.BadRequest(new { message = "PDF file is empty." });

        await using var stream = file.OpenReadStream();
        var result = await service.UploadAsync(
            stream,
            file.FileName,
            request.Name,
            file.Length,
            context.User.GetUserId(),
            ct);

        return Results.Created($"/pdf-templates/{result.Id}", result);
    }

    private static async Task<IResult> GetAvailableAsync(
        IPdfTemplateService service,
        CancellationToken ct)
    {
        var templates = await service.GetAvailableAsync(ct);
        return Results.Ok(templates);
    }

    private static async Task<IResult> GetCandidateFieldsAsync(IPdfTemplateService service)
    {
        var fields = await service.GetCandidateFieldsAsync();
        return Results.Ok(fields);
    }

    private static async Task<IResult> SaveMappingsAsync(
        int templateId,
        [FromBody] SavePdfTemplateMappingsRequest request,
        IPdfTemplateService service,
        CancellationToken ct)
    {
        var template = await service.SaveMappingsAsync(templateId, request, ct);
        return Results.Ok(template);
    }

    private static async Task<IResult> GenerateAsync(
        int templateId,
        int candidateId,
        IPdfTemplateService service,
        CancellationToken ct)
    {
        var result = await service.GenerateAsync(templateId, candidateId, ct);
        return Results.File(result.Content, result.ContentType, result.FileName);
    }

    private static async Task<IResult> DeleteAsync(
        int templateId,
        IPdfTemplateService service,
        CancellationToken ct)
    {
        await service.DeleteAsync(templateId, ct);
        return Results.NoContent();
    }
}

/// <summary>
/// </summary>
public class UploadPdfTemplateRequest
{
    /// <summary>
    /// </summary>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Optional template display name.
    /// </summary>
    public string? Name { get; set; }
}
