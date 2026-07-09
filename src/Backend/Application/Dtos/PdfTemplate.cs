namespace Application.Dtos;

public class PdfTemplateDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public List<PdfTemplateFieldDto> Fields { get; set; } = new();
    public List<PdfTemplateFieldMappingDto> FieldMappings { get; set; } = new();
}

public class PdfTemplateUploadResultDto : PdfTemplateDto
{
}

public class PdfTemplateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
}

public class PdfTemplateFieldMappingDto
{
    public string PdfFieldName { get; set; } = string.Empty;
    public string CandidateFieldKey { get; set; } = string.Empty;
}

public class CandidatePdfFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class SavePdfTemplateMappingsRequest
{
    public List<PdfTemplateFieldMappingRequest> Mappings { get; set; } = new();
}

public class PdfTemplateFieldMappingRequest
{
    public string PdfFieldName { get; set; } = string.Empty;
    public string CandidateFieldKey { get; set; } = string.Empty;
}

public class GeneratedPdfDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public byte[] Content { get; set; } = [];
}
