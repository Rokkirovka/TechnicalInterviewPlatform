using Domain.Entities;
using Infrastructure.Auth.Entities;
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
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<InterviewStage> InterviewStages { get; set; }
    public DbSet<Competency> Competencies { get; set; }
    public DbSet<VacancyCompetency> VacancyCompetencies { get; set; }
    public DbSet<CompetencyScore> CompetencyScores { get; set; }
    
    public DbSet<UserDeletionLog> UserDeletionLogs { get; set; }
    public DbSet<CandidateDeletionLog> CandidateDeletionLogs { get; set; }
    public DbSet<SkillDeletionLog> SkillDeletionLogs { get; set; }
    public DbSet<VacancyDeletionLog> VacancyDeletionLogs { get; set; }
    public DbSet<InterviewDeletionLog> InterviewDeletionLogs { get; set; }
    public DbSet<CommentDeletionLog> CommentDeletionLogs { get; set; }
    public DbSet<CompetencyDeletionLog> CompetencyDeletionLogs { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

        modelBuilder.Entity<VacancyCompetency>()
            .HasKey(vc => new { vc.VacancyId, vc.CompetencyId });

        modelBuilder.Entity<VacancyCompetency>()
            .HasOne(vc => vc.Vacancy)
            .WithMany(v => v.VacancyCompetencies)
            .HasForeignKey(vc => vc.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VacancyCompetency>()
            .HasOne(vc => vc.Competency)
            .WithMany(c => c.VacancyCompetencies)
            .HasForeignKey(vc => vc.CompetencyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Candidate)
            .WithMany(c => c.Interviews)
            .HasForeignKey(i => i.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Vacancy)
            .WithMany(v => v.Interviews)
            .HasForeignKey(i => i.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.AssignedToUser)
            .WithMany()
            .HasForeignKey(i => i.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.DecidedByUser)
            .WithMany()
            .HasForeignKey(i => i.DecidedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Interview)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
        
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();
        
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.TokenHash)
            .IsUnique();

        ConfigureDeletionLog<UserDeletionLog, User>(modelBuilder);
        ConfigureDeletionLog<CandidateDeletionLog, Candidate>(modelBuilder);
        ConfigureDeletionLog<SkillDeletionLog, Skill>(modelBuilder);
        ConfigureDeletionLog<VacancyDeletionLog, Vacancy>(modelBuilder);
        ConfigureDeletionLog<InterviewDeletionLog, Interview>(modelBuilder);
        ConfigureDeletionLog<CommentDeletionLog, Comment>(modelBuilder);
        ConfigureDeletionLog<CompetencyDeletionLog, Competency>(modelBuilder);
    }

    private void ConfigureDeletionLog<TLog, TEntity>(ModelBuilder modelBuilder)
        where TLog : DeletionLogBase<TEntity>
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TLog>()
            .HasOne(l => l.DeletedByUser)
            .WithMany()
            .HasForeignKey(l => l.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TLog>()
            .HasOne(l => l.Entity)
            .WithMany()
            .HasForeignKey(l => l.EntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}