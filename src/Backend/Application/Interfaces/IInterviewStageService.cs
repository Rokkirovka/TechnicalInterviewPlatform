using Application.Dtos;

namespace Application.Interfaces;

public interface IInterviewStageService : IBaseService<InterviewStageDto, CreateInterviewStageRequest, UpdateInterviewStageRequest>
{
    Task<IReadOnlyList<InterviewStageDto>> GetByInterviewIdAsync(int interviewId);
}