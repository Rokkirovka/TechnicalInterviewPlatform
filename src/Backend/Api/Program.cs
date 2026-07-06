using Api.Extensions;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.AddExceptionHandler();
builder.AddSwagger();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IDeletionLogRepository<>), typeof(DeletionLogRepository<>));

builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IInterviewStageService, InterviewStageService>();
builder.Services.AddScoped<ICompetencyScoreService, CompetencyScoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "App API");
    options.RoutePrefix = "swagger";
});

app.Run();
