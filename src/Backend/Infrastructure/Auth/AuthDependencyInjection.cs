using Application;
using Application.Interfaces;
using Infrastructure.Auth.Helpers;
using Infrastructure.Auth.Options;
using Infrastructure.Auth.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RefreshTokenRepository = Infrastructure.Auth.Repositories.RefreshTokenRepository;

namespace Infrastructure.Auth;

public static class AuthDependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration
        )
    {
        services.AddSingleton(configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<PasswordHasher>();
        services.AddScoped<TokenHasher>();

        return services;
    }
}
