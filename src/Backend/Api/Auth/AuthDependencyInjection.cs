using Api.Auth.Helpers;
using Api.Auth.Options;
using Api.Auth.Services;
using Application.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Repositories;
using RefreshTokenRepository = Infrastructure.Auth.RefreshTokenRepository;

namespace Api.Auth;

/// <summary>
/// 
/// </summary>
public static class AuthDependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration
        )
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
