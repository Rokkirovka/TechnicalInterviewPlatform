using Application.Dtos;

namespace Application.Interfaces;

public interface ICompetencyService : IBaseService<CompetencyDto, CreateCompetencyRequest, UpdateCompetencyRequest>;