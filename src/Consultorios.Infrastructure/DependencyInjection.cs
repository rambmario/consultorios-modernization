using Consultorios.Application.Abstractions;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consultorios.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AmbientTenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<AmbientTenantContext>());

        var connectionString = ResolveConnectionString(configuration);
        services.AddDbContext<ConsultoriosDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IConsultoriosDbContext>(sp => sp.GetRequiredService<ConsultoriosDbContext>());

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
