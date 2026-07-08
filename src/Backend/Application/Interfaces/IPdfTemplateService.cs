using Application.Dtos;

namespace Application.Interfaces;

public interface IPdfTemplateService
{
    Task<PdfTemplateUploadResultDto> UploadAsync(
        Stream file,
        string originalFileName,
        string? templateName,
        long fileSize,
        int uploadedByUserId,
        CancellationToken ct);

    Task<IReadOnlyList<PdfTemplateDto>> GetAvailableAsync(CancellationToken ct);
    Task<IReadOnlyList<CandidatePdfFieldDto>> GetCandidateFieldsAsync();
    Task<PdfTemplateDto> SaveMappingsAsync(int templateId, SavePdfTemplateMappingsRequest request, CancellationToken ct);
    Task<GeneratedPdfDto> GenerateAsync(int templateId, int candidateId, CancellationToken ct);
    Task DeleteAsync(int templateId, CancellationToken ct);
}
