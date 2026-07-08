using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CompetencyService(
    ICompetencyRepository competencyRepository,
    IMapper mapper) : ICompetencyService
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
        return mapper.Map<CompetencyDto>(competency);
    }

    public async Task<CompetencyDto> UpdateAsync(UpdateCompetencyRequest request)
    {
        var competency = mapper.Map<Competency>(request);
        await competencyRepository.UpdateAsync(competency);
        return mapper.Map<CompetencyDto>(competency);
    }
}