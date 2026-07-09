using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PdfTemplateRepository(ApplicationDbContext context) : IPdfTemplateRepository
{
    public async Task<PdfTemplate?> GetByIdWithDetailsAsync(int id, CancellationToken ct)
    {
        return await context.PdfTemplates
            .Include(t => t.Fields)
            .Include(t => t.FieldMappings)
            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null, ct);
    }

    public async Task<IReadOnlyList<PdfTemplate>> GetAliveWithDetailsAsync(CancellationToken ct)
    {
        return await context.PdfTemplates
            .Include(t => t.Fields)
            .Include(t => t.FieldMappings)
            .Where(t => t.DeletedAt == null)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);
    }
}
