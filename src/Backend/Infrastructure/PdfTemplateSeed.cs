using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public static class PdfTemplateSeed
{
    private const string PdfContentType = "application/pdf";
    private const string SeedDirectory = "SeedFiles/DocumentTemplates";

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> TemplateMappings =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Карточка кандидата.pdf"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ФИО"] = "fullName",
                ["Телефон"] = "phone",
                ["Город"] = "city",
                ["Образование"] = "education",
                ["Опыт работы"] = "previousJob",
                ["Навыки"] = "candidateSkills"
            },
            ["Журнал собеседования.pdf"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ФИО"] = "fullName",
                ["Телефон"] = "phone",
                ["Город"] = "city",
                ["Вакансия"] = "vacancyTitle",
                ["Дата"] = "interviewDate",
                ["Матрица"] = "competencyScores",
                ["Заметки"] = "interviewComments"
            },
            ["Отказ.pdf"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ФИО"] = "fullName",
                ["Телефон"] = "phone",
                ["Город"] = "city",
                ["Предыдущая работа"] = "previousJob"
            },
            ["Принятие на работу.pdf"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ФИО"] = "fullName",
                ["Телефон"] = "phone",
                ["Город"] = "city",
                ["Образование"] = "education",
                ["Предыдущая работа"] = "previousJob"
            }
        };

    public static async Task SeedAsync(
        ApplicationDbContext context,
        IObjectStorageService objectStorage,
        IPdfFormService pdfFormService,
        CancellationToken ct = default)
    {
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Login == "admin", ct)
            ?? throw new InvalidOperationException("Admin user is required before PDF template seed.");

        var seedDirectory = Path.Combine(AppContext.BaseDirectory, SeedDirectory);
        if (!Directory.Exists(seedDirectory))
            return;

        foreach (var filePath in Directory.GetFiles(seedDirectory, "*.pdf").OrderBy(Path.GetFileName))
        {
            await SeedTemplateAsync(context, objectStorage, pdfFormService, admin.Id, filePath, ct);
        }
    }

    private static async Task SeedTemplateAsync(
        ApplicationDbContext context,
        IObjectStorageService objectStorage,
        IPdfFormService pdfFormService,
        int uploadedByUserId,
        string filePath,
        CancellationToken ct)
    {
        var fileName = Path.GetFileName(filePath);
        var templateName = Path.GetFileNameWithoutExtension(filePath);
        var objectName = $"seed/pdf-templates/{fileName}";
        var fileInfo = new FileInfo(filePath);

        await using (var uploadStream = File.OpenRead(filePath))
        {
            await objectStorage.PutAsync(objectName, uploadStream, fileInfo.Length, PdfContentType, ct);
        }

        IReadOnlyList<Application.Dtos.PdfTemplateFieldDto> fields;
        await using (var readStream = File.OpenRead(filePath))
        {
            fields = pdfFormService.GetFields(readStream);
        }

        if (fields.Count == 0)
            throw new InvalidOperationException($"Seed PDF template '{fileName}' does not contain fillable form fields.");

        var template = await context.PdfTemplates
            .Include(t => t.Fields)
            .Include(t => t.FieldMappings)
            .FirstOrDefaultAsync(t => t.OriginalFileName == fileName, ct);

        if (template == null)
        {
            template = new PdfTemplate
            {
                CreatedAt = DateTime.UtcNow,
                UploadedByUserId = uploadedByUserId
            };
            context.PdfTemplates.Add(template);
        }
        else
        {
            context.PdfTemplateFields.RemoveRange(template.Fields);
            context.PdfTemplateFieldMappings.RemoveRange(template.FieldMappings);
            template.Fields.Clear();
            template.FieldMappings.Clear();
            await context.SaveChangesAsync(ct);
        }

        template.Name = templateName;
        template.OriginalFileName = fileName;
        template.ObjectName = objectName;
        template.ContentType = PdfContentType;
        template.FileSize = fileInfo.Length;
        template.UpdatedAt = DateTime.UtcNow;

        foreach (var field in fields)
        {
            template.Fields.Add(new PdfTemplateField
            {
                Name = field.Name,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                IsReadOnly = field.IsReadOnly
            });
        }

        if (TemplateMappings.TryGetValue(fileName, out var mappings))
        {
            var fieldNames = fields.Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in mappings.Where(mapping => fieldNames.Contains(mapping.Key)))
            {
                template.FieldMappings.Add(new PdfTemplateFieldMapping
                {
                    PdfFieldName = mapping.Key,
                    CandidateFieldKey = mapping.Value
                });
            }
        }

        await context.SaveChangesAsync(ct);
    }
}
