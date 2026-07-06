using Application.Dtos;

namespace Application.Interfaces;

public interface ICommentService : IBaseService<CommentDto, CreateCommentRequest, UpdateCommentRequest>
{
    Task<IReadOnlyList<CommentDto>> GetByInterviewIdAsync(int interviewId);
}