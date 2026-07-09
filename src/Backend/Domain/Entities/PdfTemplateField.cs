namespace Domain.Entities;

public class PdfTemplateField : BaseEntity
{
    public int PdfTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }

    public virtual PdfTemplate PdfTemplate { get; set; } = null!;
}
