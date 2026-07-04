namespace Domain.Entities;

public abstract class DeletionLogBase<TEntity> where TEntity : BaseEntity
{
    public int Id { get; set; }
    public int DeletedByUserId { get; set; }
    public int EntityId { get; set; }
    public string? DeletionReason { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User DeletedByUser { get; set; } = null!;
    public virtual TEntity? Entity { get; set; }
}

public class UserDeletionLog : DeletionLogBase<User>
{
}

public class CandidateDeletionLog : DeletionLogBase<Candidate>
{
}

public class SkillDeletionLog : DeletionLogBase<Skill>
{
}

public class VacancyDeletionLog : DeletionLogBase<Vacancy>
{
}

public class InterviewDeletionLog : DeletionLogBase<Interview>
{
}

public class CommentDeletionLog : DeletionLogBase<Comment>
{
}

public class CompetencyDeletionLog : DeletionLogBase<Competency>
{
}