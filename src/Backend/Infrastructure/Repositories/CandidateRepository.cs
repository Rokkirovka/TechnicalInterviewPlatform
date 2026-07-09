using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CandidateRepository(ApplicationDbContext context)
    : BaseRepository<Candidate>(context), ICandidateRepository
{
    public async Task<IReadOnlyList<Candidate>> SearchAsync(string? search, bool showArchived)
    {
        var query = search is not null 
            ? DbSet.Where(c =>
                EF.Functions.ILike(c.FirstName, $"%{search}%") ||
                EF.Functions.ILike(c.LastName, $"%{search}%") ||
                EF.Functions.ILike(c.MiddleName, $"%{search}%")) 
            : DbSet;
        
        query = showArchived
            ? query.Where(c => c.DeletedAt != null)
            : query.Where(c => c.DeletedAt == null);
        
        return await query
            .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
            .Include(c => c.Interviews)
            .ToListAsync();
    }

    public override async Task UpdateAsync(Candidate entity)
    {
        var dbEntity = await DbSet.FindAsync(entity.Id);
        if (dbEntity == null) return;
        dbEntity.UpdatedAt = DateTime.UtcNow;

        dbEntity.FirstName = entity.FirstName;
        dbEntity.MiddleName = entity.MiddleName;
        dbEntity.LastName = entity.LastName;
        dbEntity.Phone = entity.Phone;
        dbEntity.City = entity.City;
        dbEntity.Education = entity.Education;
        dbEntity.PreviousJob = entity.PreviousJob;

        foreach (var cs in dbEntity.CandidateSkills)
        {
            if (!entity.CandidateSkills.Any(s => s.SkillId == cs.SkillId))
            {
                dbEntity.CandidateSkills.Remove(cs);
            }
        }
        foreach (var cs in entity.CandidateSkills)
        {
            if (!dbEntity.CandidateSkills.Any(s => s.SkillId == cs.SkillId))
            {
                dbEntity.CandidateSkills.Add(cs);
            }
        }

        await SaveChangesAsync();
    }

    public override Task<Candidate?> GetByIdAsync(int id)
    {
        return DbSet
            .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
            .Include(c => c.Interviews)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}