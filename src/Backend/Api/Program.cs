using Api.Endpoints;
using Api.Extensions;
using Infrastructure.Auth.Helpers;
using Infrastructure.Data;
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
