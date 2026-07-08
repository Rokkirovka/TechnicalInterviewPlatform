using Application.Dtos;

namespace Application.Interfaces;

public interface ISkillService
{
    Task<IReadOnlyList<SkillDto>> GetAllAsync();
    Task<SkillDto> CreateAsync(CreateSkillRequest request);
    Task<SkillDto> UpdateAsync(int id, UpdateSkillRequest request);
}