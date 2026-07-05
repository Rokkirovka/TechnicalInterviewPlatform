using Application.Dtos;

namespace Application.Interfaces;

public interface ICandidateService : IBaseService<CandidateDto, CreateCandidateRequest, UpdateCandidateRequest>;