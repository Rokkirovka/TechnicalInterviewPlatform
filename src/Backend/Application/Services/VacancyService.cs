using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class VacancyService(
    IVacancyRepository vacancyRepository,
    ICompetencyRepository competencyRepository,
    IDeletionLogRepository<Vacancy> deletionLogRepository,
    IMapper mapper,
    ILogger<VacancyService> logger) : IVacancyService
{
    public async Task<IReadOnlyList<VacancyDto>> SearchAsync(string? search, bool showArchived)
    {
        var vacancies = await vacancyRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<VacancyDto>>(vacancies);
    }
    
    public async Task<VacancyDto> GetByIdAsync(int id)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(id);
        if (vacancy == null) throw new KeyNotFoundException($"Вакансия с id {id} не найдена");
        return mapper.Map<VacancyDto>(vacancy);
    }

    public async Task<VacancyDto> CreateAsync(CreateVacancyRequest request)
    {
        var vacancy = mapper.Map<Vacancy>(request);

        if (request.CompetencyIds.Count != 0)
        {
            var competencies = await competencyRepository.GetByIdsAsync(request.CompetencyIds);

            vacancy.VacancyCompetencies = competencies
                .Select(c => new VacancyCompetency
                {
                    CompetencyId = c.Id,
                    VacancyId = vacancy.Id
                }).ToList();
        }

        await vacancyRepository.AddAsync(vacancy);
        var result = mapper.Map<VacancyDto>(vacancy);
        
        logger.LogInformation("вакансия {VacancyId} была создана", result.Id);
        
        return result;
    }

    public async Task<VacancyDto> UpdateAsync(int id, UpdateVacancyRequest request)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(id);
        if (vacancy == null)
            throw new KeyNotFoundException($"Вакансия с id {id} не найдена");


        mapper.Map(request, vacancy);

        vacancy.VacancyCompetencies.Clear();

        if (request.CompetencyIds.Count != 0)
        {
            var competencies = await competencyRepository.GetByIdsAsync(request.CompetencyIds);

            vacancy.VacancyCompetencies = competencies
                .Select(c => new VacancyCompetency
                {
                    CompetencyId = c.Id,
                    VacancyId = vacancy.Id
                }).ToList();
        }

        await vacancyRepository.UpdateAsync(vacancy);
        var result = mapper.Map<VacancyDto>(vacancy);
        
        logger.LogInformation("вакансия {VacancyId} была обновлена", result.Id);
        
        return result;
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var vacancy = await vacancyRepository.GetByIdAsync(id);
        if (vacancy == null) throw new KeyNotFoundException($"Вакансия с id {id} не найдена");

        vacancy.DeletedAt = DateTime.UtcNow;
        await vacancyRepository.UpdateAsync(vacancy);

        await deletionLogRepository.AddAsync(vacancy, archivedByUserId, reason);
        
        logger.LogInformation("вакансия {VacancyId} была архивирована", id);
    }

    public async Task RestoreAsync(int id)
    {
        var vacancy = await vacancyRepository.GetByIdAsync(id);
        if (vacancy == null) throw new KeyNotFoundException($"Вакансия с id {id} не найдена");
        vacancy.DeletedAt = null;
        await vacancyRepository.UpdateAsync(vacancy);
        
        logger.LogInformation("вакансия {VacancyId} была восстановлена из архива", id);
    }
}