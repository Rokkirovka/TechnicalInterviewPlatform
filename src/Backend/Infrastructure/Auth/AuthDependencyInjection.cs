using Application;
using Domain;
using Infrastructure.Data.Repositories;
using Infrastructure.Auth.Helpers;
using Infrastructure.Auth.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth;

public static class AuthDependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration
        )
    {
        services.AddSingleton(
            configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<PasswordHasher>();
        services.AddScoped<TokenHasher>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
