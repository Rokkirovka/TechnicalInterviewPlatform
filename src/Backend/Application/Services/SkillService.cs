using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SkillService(
    ISkillRepository skillRepository,
    IMapper mapper,
    ILogger<SkillService> logger) : ISkillService
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
        var result = mapper.Map<SkillDto>(skill);

        logger.LogInformation("навык {SkillId} был создан", result.Id);

        return result;
    }

    public async Task<SkillDto> UpdateAsync(int id, UpdateSkillRequest request)
    {
        var skill = await skillRepository.GetByIdAsync(id);
        if (skill == null) throw new Exception($"Навык с id {id} не найден");
        mapper.Map(request, skill);
        await skillRepository.UpdateAsync(skill);
        var result = mapper.Map<SkillDto>(skill);

        logger.LogInformation("навык {SkillId} был обновлён", result.Id);

        return result;
    }
}