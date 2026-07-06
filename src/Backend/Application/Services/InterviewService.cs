using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class InterviewService(
    IRepository<Interview> repository,
    IDeletionLogRepository<Interview> deletionLogRepository,
    IRepository<Candidate> candidateRepository,
    IRepository<Vacancy> vacancyRepository,
    IRepository<User> userRepository,
    IMapper mapper)
    : BaseService<Interview, InterviewDto, CreateInterviewRequest, UpdateInterviewRequest>(
        repository,
        deletionLogRepository,
        mapper),
      IInterviewService
{
    public override async Task<InterviewDto> CreateAsync(CreateInterviewRequest request)
    {
        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate == null)
            throw new Exception($"Кандидат с id {request.CandidateId} не найден");

        var vacancy = await vacancyRepository.GetByIdAsync(request.VacancyId);
        if (vacancy == null)
            throw new Exception($"Вакансия с id {request.VacancyId} не найдена");

        var user = await userRepository.GetByIdAsync(request.CreatedByUserId);
        if (user == null)
            throw new Exception($"Пользователь с id {request.CreatedByUserId} не найден");

        var interview = Mapper.Map<Interview>(request);
        interview.Status = InterviewStatus.Scheduled;

        var result = await Repository.AddAsync(interview);
        return Mapper.Map<InterviewDto>(result);
    }

    public override async Task<InterviewDto> UpdateAsync(UpdateInterviewRequest request)
    {
        var interview = await Repository.GetByIdAsync(request.Id);
        if (interview == null)
            throw new Exception($"Интервью с id {request.Id} не найдено");

        Mapper.Map(request, interview);
        await Repository.UpdateAsync(interview);
        return Mapper.Map<InterviewDto>(interview);
    }

    public async Task UpdateStatusAsync(UpdateInterviewStatusRequest request)
    {
        var interview = await Repository.GetByIdAsync(request.Id);
        if (interview == null)
            throw new Exception($"Интервью с id {request.Id} не найдено");

        interview.Status = request.Status;
        
        if (request.DecidedByUserId.HasValue)
        {
            interview.DecidedByUserId = request.DecidedByUserId;
        }

        await Repository.UpdateAsync(interview);
    }
}