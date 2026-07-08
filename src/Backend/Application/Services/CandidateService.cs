using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CandidateService(
    ICandidateRepository candidateRepository,
    ISkillRepository skillRepository,
    IDeletionLogRepository<Candidate> deletionLogRepository,
    IMapper mapper) : ICandidateService
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
        if (candidate == null) throw new Exception($"Кандидат с id {id} не найден");
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
        return mapper.Map<CandidateDto>(candidate);
    }

    public async Task<CandidateDto> UpdateAsync(UpdateCandidateRequest request)
    {
        var candidate = await candidateRepository.GetByIdAsync(request.Id);
        if (candidate == null) throw new Exception($"Кандидат с id {request.Id} не найден");

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
        return mapper.Map<CandidateDto>(candidate);
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new Exception($"Кандидат с id {id} не найден");

        candidate.DeletedAt = DateTime.UtcNow;
        await candidateRepository.UpdateAsync(candidate);

        await deletionLogRepository.AddAsync(candidate, archivedByUserId, reason);
    }

    public async Task RestoreAsync(int id)
    {
        var candidate = await candidateRepository.GetByIdAsync(id);
        if (candidate == null) throw new Exception($"Кандидат с id {id} не найден");

        candidate.DeletedAt = null;
        await candidateRepository.UpdateAsync(candidate);
    }
}