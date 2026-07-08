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

    public async Task<CompetencyDto> UpdateAsync(int id, UpdateCompetencyRequest request)
    {
        var competency = await competencyRepository.GetByIdAsync(id);
        if (competency == null) throw new Exception($"Компетенция с id {id} не найдена");
        mapper.Map(request, competency);
        await competencyRepository.UpdateAsync(competency);
        return mapper.Map<CompetencyDto>(competency);
    }
}