using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CompetencyService(
    ICompetencyRepository competencyRepository,
    IMapper mapper,
    ILogger<CompetencyService> logger) : ICompetencyService
{
    public async Task<IReadOnlyList<CompetencyDto>> GetAllAsync()
    {
        var competencies = await competencyRepository.GetAllAsync();
        return mapper.Map<IReadOnlyList<CompetencyDto>>(competencies);
    }

    public async Task<CompetencyDto> CreateAsync(CreateCompetencyRequest request)
    {
        var competency = mapper.Map<Competency>(request);
        await competencyRepository.AddAsync(competency);
        var result = mapper.Map<CompetencyDto>(competency);

        logger.LogInformation("компетенция {CompetencyId} была создана", result.Id);

        return result;
    }

    public async Task<CompetencyDto> UpdateAsync(int id, UpdateCompetencyRequest request)
    {
        var competency = await competencyRepository.GetByIdAsync(id);
        if (competency == null) throw new Exception($"Компетенция с id {id} не найдена");
        mapper.Map(request, competency);
        await competencyRepository.UpdateAsync(competency);
        var result = mapper.Map<CompetencyDto>(competency);

        logger.LogInformation("компетенция {CompetencyId} была обновлена", result.Id);

        return result;
    }
}