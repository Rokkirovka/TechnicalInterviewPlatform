using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CommentService(
    IRepository<Comment> repository,
    IDeletionLogRepository<Comment> deletionLogRepository,
    IRepository<Interview> interviewRepository,
    IRepository<User> userRepository,
    IMapper mapper)
    : BaseService<Comment, CommentDto, CreateCommentRequest, UpdateCommentRequest>(
        repository,
        deletionLogRepository,
        mapper),
      ICommentService
{
    public async Task<IReadOnlyList<CommentDto>> GetByInterviewIdAsync(int interviewId)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {interviewId} не найдено");

        var comments = await Repository.AllAliveAsync();
        var filteredComments = comments.Where(c => c.InterviewId == interviewId).ToList();
        
        return Mapper.Map<IReadOnlyList<CommentDto>>(filteredComments);
    }

    public override async Task<CommentDto> CreateAsync(CreateCommentRequest request)
    {
        var interview = await interviewRepository.GetByIdAsync(request.InterviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {request.InterviewId} не найдено");

        var author = await userRepository.GetByIdAsync(request.AuthorId);
        if (author == null)
            throw new Exception($"Пользователь с id {request.AuthorId} не найден");

        var comment = Mapper.Map<Comment>(request);
        var result = await Repository.AddAsync(comment);
        return Mapper.Map<CommentDto>(result);
    }

    public override async Task<CommentDto> UpdateAsync(UpdateCommentRequest request)
    {
        var comment = await Repository.GetByIdAsync(request.Id);
        if (comment == null)
            throw new Exception($"Комментарий с id {request.Id} не найден");

        comment.Content = request.Content;
        await Repository.UpdateAsync(comment);
        return Mapper.Map<CommentDto>(comment);
    }
}