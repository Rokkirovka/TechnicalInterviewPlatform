using System.Reflection;
using Microsoft.OpenApi;

namespace Api.Extensions;

/// <summary>
/// 
/// </summary>
public static class SwaggerExtension
{
    /// <summary>
    /// Конфигурирует настройку Swagger в приложении.
    /// Регистрирует сервисы для документирования API, добавляет комментарии из XML файлов и настраивает схемы безопасности JWT.
    /// </summary>
    /// <param name="builder">Экземпляр WebApplicationBuilder, содержащий контекст сборки приложения.</param>
    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

            options.IncludeXmlComments(xmlPath);

            var securitySchemeId = builder.Configuration["Swagger:Definition:Id"] ?? "JWT";
            
            options.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme()
            {
                Name = builder.Configuration["Swagger:Definition:Name"],
                Description = builder.Configuration["Swagger:Definition:Description"],
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeId)] = []
            });
        });
    }
}
