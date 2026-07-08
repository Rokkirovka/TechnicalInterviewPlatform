using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using System.Text;

namespace Application.Services;

public class PdfTemplateService(
    IRepository<PdfTemplate> templateRepository,
    IPdfTemplateRepository pdfTemplateRepository,
    IRepository<Candidate> candidateRepository,
    IObjectStorageService objectStorage,
    IPdfFormService pdfFormService)
    : IPdfTemplateService
{
    private const string PdfContentType = "application/pdf";

    private static readonly IReadOnlyList<CandidatePdfFieldDto> CandidateFields =
    [
        new() { Key = "fullName", DisplayName = "Full name" },
        new() { Key = "phone", DisplayName = "Phone" },
        new() { Key = "city", DisplayName = "City" },
        new() { Key = "education", DisplayName = "Education" },
        new() { Key = "previousJob", DisplayName = "Previous job" }
    ];

    public async Task<PdfTemplateUploadResultDto> UploadAsync(
        Stream file,
        string originalFileName,
        string? templateName,
        long fileSize,
        int uploadedByUserId,
        CancellationToken ct)
    {
        if (fileSize <= 0)
            throw new InvalidOperationException("PDF file is empty.");

        if (!originalFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only PDF files are supported.");

        await using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer, ct);
        buffer.Position = 0;

        var fields = pdfFormService.GetFields(buffer);
        if (fields.Count == 0)
            throw new InvalidOperationException("PDF template must contain fillable form fields.");

        var objectName = $"pdf-templates/{Guid.NewGuid():N}.pdf";
        buffer.Position = 0;
        await objectStorage.PutAsync(objectName, buffer, buffer.Length, PdfContentType, ct);

        var template = new PdfTemplate
        {
            Name = string.IsNullOrWhiteSpace(templateName)
                ? Path.GetFileNameWithoutExtension(originalFileName)
                : templateName.Trim(),
            OriginalFileName = originalFileName,
            ObjectName = objectName,
            ContentType = PdfContentType,
            FileSize = buffer.Length,
            UploadedByUserId = uploadedByUserId,
            Fields = fields.Select(field => new PdfTemplateField
            {
                Name = field.Name,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                IsReadOnly = field.IsReadOnly
            }).ToList()
        };

        var saved = await templateRepository.AddAsync(template);
        return ToUploadResult(saved);
    }

    public async Task<IReadOnlyList<PdfTemplateDto>> GetAvailableAsync(CancellationToken ct)
    {
        var templates = await pdfTemplateRepository.GetAliveWithDetailsAsync(ct);
        return templates.Select(ToDto).ToList();
    }

    public Task<IReadOnlyList<CandidatePdfFieldDto>> GetCandidateFieldsAsync()
    {
        return Task.FromResult(CandidateFields);
    }

    public async Task<PdfTemplateDto> SaveMappingsAsync(
        int templateId,
        SavePdfTemplateMappingsRequest request,
        CancellationToken ct)
    {
        var template = await pdfTemplateRepository.GetByIdWithDetailsAsync(templateId, ct)
            ?? throw new InvalidOperationException($"PDF template with id {templateId} was not found.");

        var allowedPdfFields = template.Fields
            .Select(f => DecodePdfName(f.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allowedCandidateFields = CandidateFields.Select(f => f.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var mappings = request.Mappings
            .Where(m => !string.IsNullOrWhiteSpace(m.PdfFieldName) && !string.IsNullOrWhiteSpace(m.CandidateFieldKey))
            .Select(m => new PdfTemplateFieldMappingRequest
            {
                PdfFieldName = m.PdfFieldName.Trim(),
                CandidateFieldKey = m.CandidateFieldKey.Trim()
            })
            .ToList();

        if (mappings.Select(m => m.PdfFieldName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != mappings.Count)
            throw new InvalidOperationException("Each PDF field can have only one mapping.");

        foreach (var mapping in mappings)
        {
            if (!allowedPdfFields.Contains(mapping.PdfFieldName))
                throw new InvalidOperationException($"PDF field '{mapping.PdfFieldName}' does not exist in template.");

            if (!allowedCandidateFields.Contains(mapping.CandidateFieldKey))
                throw new InvalidOperationException($"Candidate field '{mapping.CandidateFieldKey}' is not allowed.");
        }

        template.FieldMappings.Clear();
        foreach (var mapping in mappings)
        {
            template.FieldMappings.Add(new PdfTemplateFieldMapping
            {
                PdfFieldName = mapping.PdfFieldName,
                CandidateFieldKey = mapping.CandidateFieldKey
            });
        }

        template.UpdatedAt = DateTime.UtcNow;
        await templateRepository.UpdateAsync(template);

        return ToDto(template);
    }

    public async Task<GeneratedPdfDto> GenerateAsync(int templateId, int candidateId, CancellationToken ct)
    {
        var template = await pdfTemplateRepository.GetByIdWithDetailsAsync(templateId, ct)
            ?? throw new InvalidOperationException($"PDF template with id {templateId} was not found.");

        if (template.FieldMappings.Count == 0)
            throw new InvalidOperationException("PDF template does not have field mappings.");

        var candidate = await candidateRepository.GetByIdAsync(candidateId)
            ?? throw new InvalidOperationException($"Candidate with id {candidateId} was not found.");

        await using var pdf = await objectStorage.GetAsync(template.ObjectName, ct);
        var values = template.FieldMappings.ToDictionary(
            mapping => mapping.PdfFieldName,
            mapping => GetCandidateValue(candidate, mapping.CandidateFieldKey),
            StringComparer.OrdinalIgnoreCase);

        return new GeneratedPdfDto
        {
            FileName = $"{SanitizeFileName(template.Name)}-{candidate.Id}.pdf",
            Content = pdfFormService.FillForm(pdf, values)
        };
    }

    public async Task DeleteAsync(int templateId, CancellationToken ct)
    {
        var template = await pdfTemplateRepository.GetByIdWithDetailsAsync(templateId, ct)
            ?? throw new InvalidOperationException($"PDF template with id {templateId} was not found.");

        await objectStorage.DeleteAsync(template.ObjectName, ct);
        await templateRepository.DeleteAsync(template);
    }

    private static string? GetCandidateValue(Candidate candidate, string key)
    {
        return key.ToLowerInvariant() switch
        {
            "fullname" => candidate.FullName,
            "phone" => candidate.Phone,
            "city" => candidate.City,
            "education" => candidate.Education,
            "previousjob" => candidate.PreviousJob,
            _ => null
        };
    }

    private static PdfTemplateUploadResultDto ToUploadResult(PdfTemplate template)
    {
        var dto = ToDto(template);
        return new PdfTemplateUploadResultDto
        {
            Id = dto.Id,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Name = dto.Name,
            OriginalFileName = dto.OriginalFileName,
            FileSize = dto.FileSize,
            Fields = dto.Fields,
            FieldMappings = dto.FieldMappings
        };
    }

    private static PdfTemplateDto ToDto(PdfTemplate template)
    {
        return new PdfTemplateDto
        {
            Id = template.Id,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            Name = template.Name,
            OriginalFileName = template.OriginalFileName,
            FileSize = template.FileSize,
            Fields = template.Fields
                .OrderBy(f => f.Name)
                .Select(f => new PdfTemplateFieldDto
                {
                    Name = DecodePdfName(f.Name),
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    IsReadOnly = f.IsReadOnly
                })
                .ToList(),
            FieldMappings = template.FieldMappings
                .OrderBy(m => m.PdfFieldName)
                .Select(m => new PdfTemplateFieldMappingDto
                {
                    PdfFieldName = DecodePdfName(m.PdfFieldName),
                    CandidateFieldKey = m.CandidateFieldKey
                })
                .ToList()
        };
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = value.Select(ch => invalidChars.Contains(ch) ? '-' : ch).ToArray();
        return new string(chars).Trim('-');
    }

    private static string DecodePdfName(string value)
    {
        if (!value.Contains('#'))
            return value;

        using var bytes = new MemoryStream();
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '#'
                && i + 2 < value.Length
                && IsHex(value[i + 1])
                && IsHex(value[i + 2]))
            {
                bytes.WriteByte(Convert.ToByte(value.Substring(i + 1, 2), 16));
                i += 2;
                continue;
            }

            bytes.Write(Encoding.UTF8.GetBytes(value[i].ToString()));
        }

        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    private static bool IsHex(char value)
    {
        return value is >= '0' and <= '9'
               || value is >= 'a' and <= 'f'
               || value is >= 'A' and <= 'F';
    }
}
