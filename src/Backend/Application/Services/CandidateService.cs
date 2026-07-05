using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CandidateService(
    IRepository<Candidate> repository,
    IDeletionLogRepository<Candidate> deletionLogRepository,
    IRepository<Skill> skillRepository,
    IMapper mapper)
    : BaseService<Candidate, CandidateDto, CreateCandidateRequest, UpdateCandidateRequest>(
        repository,
        deletionLogRepository,
        mapper),
      ICandidateService
{
    public override async Task<CandidateDto> CreateAsync(CreateCandidateRequest request)
    {
        var candidate = Mapper.Map<Candidate>(request);
        
        if (request.Skills.Any())
        {
            var skillIds = request.Skills.Select(s => s.SkillId).ToList();
            var allSkills = await skillRepository.AllAliveAsync();
            var existingSkills = allSkills.Where(s => skillIds.Contains(s.Id)).ToList();
            
            candidate.CandidateSkills = existingSkills.Select(s => new CandidateSkill
            {
                SkillId = s.Id,
                Level = request.Skills.First(x => x.SkillId == s.Id).Level
            }).ToList();
        }

        var result = await Repository.AddAsync(candidate);
        return Mapper.Map<CandidateDto>(result);
    }

    public override async Task<CandidateDto> UpdateAsync(UpdateCandidateRequest request)
    {
        var candidate = await Repository.GetByIdAsync(request.Id);
        if (candidate == null)
            throw new Exception($"Кандидат с id {request.Id} не найден");

        Mapper.Map(request, candidate);
        
        candidate.CandidateSkills.Clear();
        
        if (request.Skills.Any())
        {
            var skillIds = request.Skills.Select(s => s.SkillId).ToList();
            var allSkills = await skillRepository.AllAliveAsync();
            var existingSkills = allSkills.Where(s => skillIds.Contains(s.Id)).ToList();
            
            candidate.CandidateSkills = existingSkills.Select(s => new CandidateSkill
            {
                SkillId = s.Id,
                Level = request.Skills.First(x => x.SkillId == s.Id).Level
            }).ToList();
        }

        await Repository.UpdateAsync(candidate);
        return Mapper.Map<CandidateDto>(candidate);
    }
}