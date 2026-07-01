using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<CandidateSkill> CandidateSkills { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<InterviewStage> InterviewStages { get; set; }
    public DbSet<Competency> Competencies { get; set; }
    public DbSet<CompetencyScore> CompetencyScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRoles"));

        modelBuilder.Entity<CandidateSkill>()
            .HasKey(cs => new { cs.CandidateId, cs.SkillId });

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.Candidate)
            .WithMany(c => c.CandidateSkills)
            .HasForeignKey(cs => cs.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CandidateSkills)
            .HasForeignKey(cs => cs.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Candidate)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Vacancy)
            .WithMany(v => v.Applications)
            .HasForeignKey(a => a.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Application)
            .WithOne(a => a.Interview)
            .HasForeignKey<Interview>(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewStage>()
            .HasOne(ist => ist.Interview)
            .WithMany(i => i.InterviewStages)
            .HasForeignKey(ist => ist.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompetencyScore>()
            .HasOne(cs => cs.Interview)
            .WithMany(i => i.CompetencyScores)
            .HasForeignKey(cs => cs.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompetencyScore>()
            .HasOne(cs => cs.Competency)
            .WithMany(c => c.CompetencyScores)
            .HasForeignKey(cs => cs.CompetencyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.DecidedByUser)
            .WithMany()
            .HasForeignKey(i => i.DecidedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}