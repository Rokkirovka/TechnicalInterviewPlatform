using Domain.Entities;

namespace Application.Interfaces;

public interface ISkillRepository : IRepository<Skill>
{
    Task<IReadOnlyList<Skill>> GetAllAsync();
    Task<IReadOnlyList<Skill>> GetByNamesAsync(List<string> names);
}