using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class VacancyService(
    IRepository<Vacancy> repository,
    IDeletionLogRepository<Vacancy> deletionLogRepository,
    IRepository<Competency> competencyRepository,
    IMapper mapper)
    : BaseService<Vacancy, VacancyDto, CreateVacancyRequest, UpdateVacancyRequest>(
        repository,
        deletionLogRepository,
        mapper),
      IVacancyService
{
    public override async Task<VacancyDto> CreateAsync(CreateVacancyRequest request)
    {
        var vacancy = Mapper.Map<Vacancy>(request);
        
        if (request.CompetencyIds.Any())
        {
            var competencies = await competencyRepository.AllAliveAsync();
            var existingCompetencies = competencies
                .Where(c => request.CompetencyIds.Contains(c.Id))
                .ToList();
            
            vacancy.VacancyCompetencies = existingCompetencies.Select(c => new VacancyCompetency
            {
                CompetencyId = c.Id
            }).ToList();
        }

        var result = await Repository.AddAsync(vacancy);
        return Mapper.Map<VacancyDto>(result);
    }

    public override async Task<VacancyDto> UpdateAsync(UpdateVacancyRequest request)
    {
        var vacancy = await Repository.GetByIdAsync(request.Id);
        if (vacancy == null)
            throw new Exception($"Вакансия с id {request.Id} не найдена");

        Mapper.Map(request, vacancy);
        
        vacancy.VacancyCompetencies.Clear();
        
        if (request.CompetencyIds.Any())
        {
            var competencies = await competencyRepository.AllAliveAsync();
            var existingCompetencies = competencies
                .Where(c => request.CompetencyIds.Contains(c.Id))
                .ToList();
            
            vacancy.VacancyCompetencies = existingCompetencies.Select(c => new VacancyCompetency
            {
                CompetencyId = c.Id
            }).ToList();
        }

        await Repository.UpdateAsync(vacancy);
        return Mapper.Map<VacancyDto>(vacancy);
    }
}