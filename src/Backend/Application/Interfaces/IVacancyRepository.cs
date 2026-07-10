using Domain.Entities;

namespace Application.Interfaces;

public interface IVacancyRepository : IRepository<Vacancy>
{
    Task<Vacancy?> GetWithCompetenciesAsync(int id);
    Task<IReadOnlyList<Vacancy>> SearchAsync(string? search, bool showArchived);
}