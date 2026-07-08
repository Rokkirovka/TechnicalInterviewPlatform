using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class VacancyService(
    IVacancyRepository vacancyRepository,
    ICompetencyRepository competencyRepository,
    IDeletionLogRepository<Vacancy> deletionLogRepository,
    IMapper mapper) : IVacancyService
{
    public async Task<IReadOnlyList<VacancyDto>> SearchAsync(string? search, bool showArchived)
    {
        var vacancies = await vacancyRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<VacancyDto>>(vacancies);
    }
    
    public async Task<VacancyDto> GetByIdAsync(int id)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(id);
        if (vacancy == null) throw new Exception($"Вакансия с id {id} не найдена");
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
        return mapper.Map<VacancyDto>(vacancy);
    }

    public async Task<VacancyDto> UpdateAsync(UpdateVacancyRequest request)
    {
        var vacancy = await vacancyRepository.GetWithCompetenciesAsync(request.Id);
        if (vacancy == null)
            throw new Exception($"Вакансия с id {request.Id} не найдена");

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
        return mapper.Map<VacancyDto>(vacancy);
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var vacancy = await vacancyRepository.GetByIdAsync(id);
        if (vacancy == null) throw new Exception($"Вакансия с id {id} не найдена");

        vacancy.DeletedAt = DateTime.UtcNow;
        await vacancyRepository.UpdateAsync(vacancy);

        await deletionLogRepository.AddAsync(vacancy, archivedByUserId, reason);
    }

    public async Task RestoreAsync(int id)
    {
        var vacancy = await vacancyRepository.GetByIdAsync(id);
        if (vacancy == null) throw new Exception($"Вакансия с id {id} не найдена");
        vacancy.DeletedAt = null;
        await vacancyRepository.UpdateAsync(vacancy);
    }
}