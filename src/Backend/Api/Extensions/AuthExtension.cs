using System.Text;
using Api.Auth;
using Api.Auth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

/// <summary>
/// 
/// </summary>
public static class AuthExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthInfrastructure(builder.Configuration);

        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var c = context.Request.Cookies[jwtSettings.AccessTokenCookieName];
                        if (c != null && context.Token == null)
                        {
                            context.Token = c;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(RoleBasedPolicies.Admin, policy =>
                policy.RequireRole("Administrator"))
            .AddPolicy(RoleBasedPolicies.Hr, policy =>
                policy.RequireRole("HumanResources"))
            .AddPolicy(RoleBasedPolicies.DecisionMaker, policy =>
                policy.RequireRole("DecisionMaker"))
            .AddPolicy(RoleBasedPolicies.AdminAndHr, policy =>
                policy.RequireRole("Administrator", "HumanResources"))
            .AddPolicy(RoleBasedPolicies.AdminAndDecisionMaker, policy =>
                policy.RequireRole("Administrator", "DecisionMaker"))
            .AddPolicy(RoleBasedPolicies.Authenticated, policy =>
                policy.RequireAuthenticatedUser());

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseAuthPipeline(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
