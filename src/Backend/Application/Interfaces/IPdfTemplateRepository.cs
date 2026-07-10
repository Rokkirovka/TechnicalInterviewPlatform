using Domain.Entities;

namespace Application.Interfaces;

public interface IPdfTemplateRepository
{
    Task<PdfTemplate?> GetByIdWithDetailsAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<PdfTemplate>> GetAliveWithDetailsAsync(CancellationToken ct);
}
