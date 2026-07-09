using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Candidate, CandidateDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.CandidateSkills))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.PreviousJob))
            .ForMember(dest => dest.Archived, opt => opt.MapFrom(src => src.DeletedAt != null));

        CreateMap<Candidate, CandidateNameDto>();

        CreateMap<CandidateSkill, CandidateSkillDto>()
            .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name));

        CreateMap<CreateCandidateRequest, Candidate>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    $"{src.LastName} {src.FirstName} {src.MiddleName}".Trim()));  

        CreateMap<UpdateCandidateRequest, Candidate>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    $"{src.LastName} {src.FirstName} {src.MiddleName}".Trim()));

        CreateMap<CreateCandidateSkillRequest, CandidateSkill>();
        CreateMap<UpdateCandidateSkillRequest, CandidateSkill>();

        CreateMap<Vacancy, VacancyDto>()
            .ForMember(dest => dest.Competencies, opt => opt.MapFrom(src => src.VacancyCompetencies))
            .ForMember(dest => dest.Archived, opt => opt.MapFrom(src => src.DeletedAt != null));

        CreateMap<VacancyCompetency, VacancyCompetencyDto>()
            .ForMember(dest => dest.CompetencyName, opt => opt.MapFrom(src => src.Competency.Name));

        CreateMap<CreateVacancyRequest, Vacancy>();
        CreateMap<UpdateVacancyRequest, Vacancy>();

        CreateMap<Skill, SkillDto>();
        CreateMap<CreateSkillRequest, Skill>();
        CreateMap<UpdateSkillRequest, Skill>();

        CreateMap<Competency, CompetencyDto>();
        CreateMap<CreateCompetencyRequest, Competency>();
        CreateMap<UpdateCompetencyRequest, Competency>();

        CreateMap<Interview, InterviewDto>()
            .ForMember(dest => dest.CandidateName,
                opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.FullName : string.Empty))
            .ForMember(dest => dest.VacancyTitle,
                opt => opt.MapFrom(src => src.Vacancy != null ? src.Vacancy.Title : string.Empty))
            .ForMember(dest => dest.CreatedByUserName,
                opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.FullName : string.Empty))
            .ForMember(dest => dest.AssignedToUserName,
                opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : string.Empty))
            .ForMember(dest => dest.DecidedByUserName,
                opt => opt.MapFrom(src => src.DecidedByUser != null ? src.DecidedByUser.FullName : string.Empty))
            .ForMember(dest => dest.Stages, opt => opt.MapFrom(src => src.InterviewStages))
            .ForMember(dest => dest.Matrix, opt => opt.MapFrom(src => src.CompetencyScores))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.Archived, opt => opt.MapFrom(src => src.DeletedAt != null))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src =>
                src.Comments.Count != 0
                    ? src.Comments.OrderByDescending(c => c.CreatedAt).Last().Content
                    : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusAsString));

        CreateMap<CreateInterviewRequest, Interview>()
            .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => src.ScheduledAt))
            .ForMember(dest => dest.InterviewStages, opt => opt.MapFrom(src => src.Stages));

        CreateMap<UpdateInterviewRequest, Interview>()
            .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => src.ScheduledAt));

        CreateMap<InterviewStage, InterviewStageDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StageName))
            .ForMember(dest => dest.StageNumber, opt => opt.MapFrom(src => src.OrderNumber));
        CreateMap<CreateInterviewStageRequest, InterviewStage>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.StageNumber))
            .ForMember(dest => dest.StageName, opt => opt.MapFrom(src => src.Name));

        CreateMap<CompetencyScore, CompetencyScoreDto>()
            .ForMember(dest => dest.CompetencyName, opt => opt.MapFrom(src => src.Competency.Name));

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : string.Empty));

        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToList()))
            .ForMember(dest => dest.Archived, opt => opt.MapFrom(src => src.DeletedAt != null))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    $"{src.LastName} {src.FirstName} {src.MiddleName}"))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    $"{src.LastName} {src.FirstName} {src.MiddleName}"))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}