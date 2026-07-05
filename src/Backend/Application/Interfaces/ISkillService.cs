using Application.Dtos;

namespace Application.Interfaces;

public interface ISkillService : IBaseService<SkillDto, CreateSkillRequest, UpdateSkillRequest>;