using Application.Dtos;

namespace Application.Interfaces;

public interface IVacancyService : IBaseService<VacancyDto, CreateVacancyRequest, UpdateVacancyRequest>;