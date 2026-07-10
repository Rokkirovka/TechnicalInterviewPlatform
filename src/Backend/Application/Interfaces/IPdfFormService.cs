using Application.Dtos;

namespace Application.Interfaces;

public interface IPdfFormService
{
    IReadOnlyList<PdfTemplateFieldDto> GetFields(Stream pdfStream);
    byte[] FillForm(Stream pdfStream, IReadOnlyDictionary<string, string?> values);
}
