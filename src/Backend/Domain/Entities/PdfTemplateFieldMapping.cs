namespace Domain.Entities;

public class PdfTemplateFieldMapping : BaseEntity
{
    public int PdfTemplateId { get; set; }
    public string PdfFieldName { get; set; } = string.Empty;
    public string CandidateFieldKey { get; set; } = string.Empty;

    public virtual PdfTemplate PdfTemplate { get; set; } = null!;
}
