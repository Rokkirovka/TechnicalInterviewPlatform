using Application.Dtos;

namespace Application.Interfaces;

public interface IInterviewService : IBaseService<InterviewDto, CreateInterviewRequest, UpdateInterviewRequest>
{
    Task UpdateStatusAsync(UpdateInterviewStatusRequest request);
}