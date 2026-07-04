namespace Domain.Entities;

public class Comment : BaseEntity
{
    public int InterviewId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;

    public virtual Interview Interview { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
}