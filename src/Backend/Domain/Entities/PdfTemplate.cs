namespace Domain.Entities;

public class PdfTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public long FileSize { get; set; }
    public int UploadedByUserId { get; set; }

    public virtual User UploadedByUser { get; set; } = null!;
    public virtual ICollection<PdfTemplateField> Fields { get; set; } = new List<PdfTemplateField>();
    public virtual ICollection<PdfTemplateFieldMapping> FieldMappings { get; set; } = new List<PdfTemplateFieldMapping>();
}
