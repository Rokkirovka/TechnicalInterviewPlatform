using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class SkillService(
    ISkillRepository skillRepository,
    IMapper mapper) : ISkillService
{
    public async Task<IReadOnlyList<SkillDto>> GetAllAsync()
    {
        var skills = await skillRepository.GetAllAsync();
        return mapper.Map<IReadOnlyList<SkillDto>>(skills);
    }

    public async Task<SkillDto> CreateAsync(CreateSkillRequest request)
    {
        var skill = mapper.Map<Skill>(request);
        await skillRepository.AddAsync(skill);
        return mapper.Map<SkillDto>(skill);
    }

    public async Task<SkillDto> UpdateAsync(UpdateSkillRequest request)
    {
        var skill = mapper.Map<Skill>(request);
        await skillRepository.UpdateAsync(skill);
        return mapper.Map<SkillDto>(skill);
    }
}