using Application.Dtos;
using Application.Interfaces;
using System.Text;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;

namespace Infrastructure.Pdf;

public class PdfSharpFormService : IPdfFormService
{
    public PdfSharpFormService()
    {
        GlobalFontSettings.FontResolver ??= new DejaVuFontResolver();
    }

    public IReadOnlyList<PdfTemplateFieldDto> GetFields(Stream pdfStream)
    {
        using var document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Modify);
        var acroForm = document.AcroForm;
        if (acroForm?.Fields is null || acroForm.Fields.Count == 0)
            return [];

        return GetFields(acroForm.Fields)
            .GroupBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(field => field.Name)
            .ToList();
    }

    public byte[] FillForm(Stream pdfStream, IReadOnlyDictionary<string, string?> values)
    {
        using var document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Modify);
        var acroForm = document.AcroForm
            ?? throw new InvalidOperationException("PDF template does not contain a fillable form.");

        acroForm.Elements.SetBoolean("/NeedAppearances", true);

        foreach (var (fieldName, value) in values)
        {
            var field = FindField(acroForm, fieldName);
            if (field is null)
                continue;

            SetFieldValue(field, value ?? string.Empty);
        }

        using var output = new MemoryStream();
        document.Save(output, false);
        return output.ToArray();
    }

    private static IEnumerable<PdfTemplateFieldDto> GetFields(PdfAcroField.PdfAcroFieldCollection fields)
    {
        foreach (var name in fields.DescendantNames)
        {
            var field = fields[name];
            if (field is null || field.HasKids)
                continue;

            yield return new PdfTemplateFieldDto
            {
                Name = DecodePdfName(field.Name),
                FieldType = GetFieldType(field),
                IsRequired = field.Flags.HasFlag(PdfAcroFieldFlags.Required),
                IsReadOnly = field.ReadOnly
            };
        }
    }

    private static void SetFieldValue(PdfAcroField field, string value)
    {
        switch (field)
        {
            case PdfTextField textField:
                textField.Text = value;
                break;
            case PdfCheckBoxField checkBoxField:
                checkBoxField.Checked = IsTruthy(value);
                break;
            default:
                field.Value = new PdfString(value);
                break;
        }
    }

    private static PdfAcroField? FindField(PdfAcroForm acroForm, string fieldName)
    {
        var directField = acroForm.Fields[fieldName];
        if (directField != null)
            return directField;

        foreach (var name in acroForm.Fields.DescendantNames)
        {
            var field = acroForm.Fields[name];
            if (field == null)
                continue;

            if (string.Equals(DecodePdfName(field.Name), fieldName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(DecodePdfName(name), fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return field;
            }
        }

        return null;
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
                var hex = value.Substring(i + 1, 2);
                bytes.WriteByte(Convert.ToByte(hex, 16));
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

    private static string GetFieldType(PdfAcroField field)
    {
        return field switch
        {
            PdfTextField => "Text",
            PdfCheckBoxField => "Checkbox",
            PdfRadioButtonField => "Radio",
            _ => field.GetType().Name
        };
    }

    private static bool IsTruthy(string value)
    {
        return value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
               || value.Equals("on", StringComparison.OrdinalIgnoreCase)
               || value == "1";
    }
}
