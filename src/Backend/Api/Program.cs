using Api.Endpoints;
using Api.Extensions;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure;
using Infrastructure.Pdf;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Minio;

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
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddMinio(configureClient =>
{
    var options = builder.Configuration.GetSection("Minio").Get<MinioOptions>() ?? new MinioOptions();
    configureClient
        .WithEndpoint(options.Endpoint)
        .WithCredentials(options.AccessKey, options.SecretKey)
        .WithSSL(options.UseSsl)
        .Build();
});

builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IInterviewStageService, InterviewStageService>();
builder.Services.AddScoped<ICompetencyScoreService, CompetencyScoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPdfTemplateRepository, PdfTemplateRepository>();
builder.Services.AddScoped<IPdfDocumentDataRepository, PdfDocumentDataRepository>();
builder.Services.AddScoped<IObjectStorageService, MinioObjectStorageService>();
builder.Services.AddScoped<IPdfFormService, PdfSharpFormService>();
builder.Services.AddScoped<IPdfTemplateService, PdfTemplateService>();
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await DbContextSeed.SeedAsync(context, passwordHasher, configuration);
    
    var objectStorage = scope.ServiceProvider.GetRequiredService<IObjectStorageService>();
    var pdfFormService = scope.ServiceProvider.GetRequiredService<IPdfFormService>();
    await PdfTemplateSeed.SeedAsync(context, objectStorage, pdfFormService);
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
app.MapPdfTemplateEndpoints();

app.Run();
