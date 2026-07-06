using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Candidate, CandidateDto>()
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.CandidateSkills));
        
        CreateMap<CandidateSkill, CandidateSkillDto>()
            .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name));

        CreateMap<CreateCandidateRequest, Candidate>();
        CreateMap<CreateCandidateSkillRequest, CandidateSkill>();

        CreateMap<UpdateCandidateRequest, Candidate>();
        CreateMap<UpdateCandidateSkillRequest, CandidateSkill>();

        CreateMap<Vacancy, VacancyDto>()
            .ForMember(dest => dest.Competencies, opt => opt.MapFrom(src => src.VacancyCompetencies));

        CreateMap<VacancyCompetency, VacancyCompetencyDto>()
            .ForMember(dest => dest.CompetencyName, opt => opt.MapFrom(src => src.Competency.Name));

        CreateMap<CreateVacancyRequest, Vacancy>();
        CreateMap<UpdateVacancyRequest, Vacancy>();

        CreateMap<Skill, SkillDto>();
        CreateMap<CreateSkillRequest, Skill>();
        CreateMap<UpdateSkillRequest, Skill>();

        CreateMap<Interview, InterviewDto>();
        CreateMap<CreateInterviewRequest, Interview>();
        CreateMap<UpdateInterviewRequest, Interview>();

        CreateMap<Competency, CompetencyDto>();
        CreateMap<CreateCompetencyRequest, Competency>();
        CreateMap<UpdateCompetencyRequest, Competency>();

        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();
        
        CreateMap<InterviewStage, InterviewStageDto>();
        CreateMap<CreateInterviewStageRequest, InterviewStage>();
        CreateMap<UpdateInterviewStageRequest, InterviewStage>();
        
        CreateMap<CompetencyScore, CompetencyScoreDto>()
            .ForMember(dest => dest.CompetencyName, opt => opt.MapFrom(src => src.Competency.Name));

        CreateMap<CreateCompetencyScoreRequest, CompetencyScore>();
        CreateMap<UpdateCompetencyScoreRequest, CompetencyScore>();
    }
}