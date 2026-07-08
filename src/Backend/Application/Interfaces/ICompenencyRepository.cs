using Domain.Entities;

namespace Application.Interfaces;

public interface ICompetencyRepository : IRepository<Competency>
{
    Task<IReadOnlyList<Competency>> GetAllAsync();
    Task<IReadOnlyList<Competency>> GetByIdsAsync(List<int> ids);
}