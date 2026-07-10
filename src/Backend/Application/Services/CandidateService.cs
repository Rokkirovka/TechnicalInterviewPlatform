using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CandidateService(
    ICandidateRepository candidateRepository,
    ISkillRepository skillRepository,
    IDeletionLogRepository<Candidate> deletionLogRepository,
    IMapper mapper,
    ILogger<CandidateService> logger) : ICandidateService
{
    public async Task<IReadOnlyList<CandidateDto>> SearchAsync(string? search, bool showArchived)
    {
        var candidates = await candidateRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<CandidateDto>>(candidates);
    }

    public async Task<IReadOnlyList<CandidateNameDto>> GetNamesAsync()
    {
        var candidates = await candidateRepository.SearchAsync(null, false);
        return mapper.Map<IReadOnlyList<CandidateNameDto>>(candidates);
    }

    public async Task<CandidateDto> GetByIdAsync(int id)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new KeyNotFoundException($"Кандидат с id {id} не найден");
        return mapper.Map<CandidateDto>(candidate);
    }

    public async Task<CandidateDto> CreateAsync(CreateCandidateRequest request)
    {
        var candidate = mapper.Map<Candidate>(request);

        if (request.Skills.Count != 0)
        {
            var skillNames = request.Skills.Select(s => s.SkillName).ToList();
            var existingSkills = await skillRepository.GetByNamesAsync(skillNames);

            foreach (var skillRequest in request.Skills)
            {
                var skill = existingSkills.FirstOrDefault(s => s.Name.ToLower() == skillRequest.SkillName.ToLower());
                if (skill != null)
                {
                    candidate.CandidateSkills.Add(new CandidateSkill
                    {
                        SkillId = skill.Id,
                        Level = skillRequest.Level
                    });
                }
            }
        }

        await candidateRepository.AddAsync(candidate);
        var result = mapper.Map<CandidateDto>(candidate);
        
        logger.LogInformation("кандидат {CandidateId} был создан", result.Id);

        return result;
    }

    public async Task<CandidateDto> UpdateAsync(int id, UpdateCandidateRequest request)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new KeyNotFoundException($"Кандидат с id {id} не найден");

        mapper.Map(request, candidate);
        candidate.CandidateSkills.Clear();

        if (request.Skills.Count != 0)
        {
            var skillNames = request.Skills.Select(s => s.SkillName).ToList();
            var existingSkills = await skillRepository.GetByNamesAsync(skillNames);

            foreach (var skillRequest in request.Skills)
            {
                var skill = existingSkills.FirstOrDefault(s => s.Name.ToLower() == skillRequest.SkillName.ToLower());
                if (skill != null)
                {
                    candidate.CandidateSkills.Add(new CandidateSkill
                    {
                        SkillId = skill.Id,
                        Level = skillRequest.Level
                    });
                }
            }
        }

        await candidateRepository.UpdateAsync(candidate);
        var result = mapper.Map<CandidateDto>(candidate);
        
        logger.LogInformation("кандидат {CandidateId} был обновлён", result.Id);
        
        return result;
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new KeyNotFoundException($"Кандидат с id {id} не найден");

        candidate.DeletedAt = DateTime.UtcNow;
        await candidateRepository.UpdateAsync(candidate);

        await deletionLogRepository.AddAsync(candidate, archivedByUserId, reason);
        
        logger.LogInformation("кандидат {CandidateId} был архивирован", candidate.Id);
    }

    public async Task RestoreAsync(int id)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new KeyNotFoundException($"Кандидат с id {id} не найден");

        candidate.DeletedAt = null;
        await candidateRepository.UpdateAsync(candidate);
        
        logger.LogInformation("кандидат {CandidateId} был восстановлен", candidate.Id);
    }
}