namespace Api.Endpoints.Requests;

/// <summary>
/// Multipart form request for uploading a fillable PDF template.
/// </summary>
public class UploadPdfTemplateRequest
{
    /// <summary>
    /// Fillable PDF file.
    /// </summary>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Optional template display name.
    /// </summary>
    public string? Name { get; set; }
}
