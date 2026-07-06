using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class InterviewStageService(
    IRepository<InterviewStage> repository,
    IDeletionLogRepository<InterviewStage> deletionLogRepository,
    IRepository<Interview> interviewRepository,
    IMapper mapper)
    : BaseService<InterviewStage, InterviewStageDto, CreateInterviewStageRequest, UpdateInterviewStageRequest>(
            repository,
            deletionLogRepository,
            mapper),
        IInterviewStageService
{
    public async Task<IReadOnlyList<InterviewStageDto>> GetByInterviewIdAsync(int interviewId)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {interviewId} не найдено");

        var stages = await Repository.AllAliveAsync();
        var filteredStages = stages
            .Where(s => s.InterviewId == interviewId)
            .OrderBy(s => s.OrderNumber)
            .ToList();

        return Mapper.Map<IReadOnlyList<InterviewStageDto>>(filteredStages);
    }

    public override async Task<InterviewStageDto> CreateAsync(CreateInterviewStageRequest request)
    {
        var interview = await interviewRepository.GetByIdAsync(request.InterviewId);
        if (interview == null)
            throw new Exception($"Интервью с id {request.InterviewId} не найдено");

        var stage = Mapper.Map<InterviewStage>(request);
        var result = await Repository.AddAsync(stage);
        return Mapper.Map<InterviewStageDto>(result);
    }
}