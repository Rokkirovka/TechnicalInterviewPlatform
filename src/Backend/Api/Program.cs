using Api.Endpoints;
using Api.Extensions;
using Infrastructure.Auth.Helpers;
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
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName!)
    ));

builder.AddAuth();

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IDeletionLogRepository<>), typeof(DeletionLogRepository<>));

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IDeletionLogRepository<>), typeof(DeletionLogRepository<>));
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ICompetencyRepository, CompetencyRepository>();
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    
    var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await DbContextSeed.SeedAsync(context, passwordHasher, configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "App API");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler(); 
app.UseAuthPipeline();
app.MapAuthEndpoints();

app.Run();
