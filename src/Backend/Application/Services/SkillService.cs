using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class SkillService(
    IRepository<Skill> repository,
    IDeletionLogRepository<Skill> deletionLogRepository,
    IMapper mapper)
    : BaseService<Skill, SkillDto, CreateSkillRequest, UpdateSkillRequest>(
            repository,
            deletionLogRepository,
            mapper),
        ISkillService;