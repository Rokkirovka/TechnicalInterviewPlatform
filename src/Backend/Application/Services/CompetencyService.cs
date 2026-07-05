using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CompetencyService(
    IRepository<Competency> repository,
    IDeletionLogRepository<Competency> deletionLogRepository,
    IMapper mapper)
    : BaseService<Competency, CompetencyDto, CreateCompetencyRequest, UpdateCompetencyRequest>(
            repository,
            deletionLogRepository,
            mapper),
        ICompetencyService;