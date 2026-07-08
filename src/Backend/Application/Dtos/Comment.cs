namespace Application.Dtos;

public class CommentDto : BaseDto
{
    public int InterviewId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class CreateCommentRequest
{
    public int InterviewId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
}