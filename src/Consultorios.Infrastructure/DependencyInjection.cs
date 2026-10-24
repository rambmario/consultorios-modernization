using System.Text;
using Consultorios.Application.Abstractions;
using Consultorios.Application.Auth;
using Consultorios.Infrastructure.Identity;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Consultorios.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AmbientTenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<AmbientTenantContext>());
        services.AddScoped<AmbientCurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<AmbientCurrentUser>());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserDirectory, UserDirectory>();
        services.AddSingleton<JwtTokenService>();

        var connectionString = ResolveConnectionString(configuration);
        services.AddDbContext<ConsultoriosDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IConsultoriosDbContext>(sp => sp.GetRequiredService<ConsultoriosDbContext>());

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ConsultoriosDbContext>()
            .AddDefaultTokenProviders();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .PostConfigure(options =>
            {
                var envKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
                if (!string.IsNullOrWhiteSpace(envKey))
                {
                    options.SigningKey = envKey;
                }
            })
            .ValidateDataAnnotations()
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.SigningKey) && o.SigningKey.Length >= 32,
                "Jwt:SigningKey must be configured with at least 32 characters (set Jwt:SigningKey or JWT_SIGNING_KEY).")
            .ValidateOnStart();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtAccessor) =>
            {
                var jwt = jwtAccessor.Value;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    internal static string ResolveConnectionString(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Consultorios");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        return DotEnv.BuildLocalPostgresConnectionString();
    }
}
