using Consultorios.Application.Abstractions;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Consultorios.Api.IntegrationTests.Infrastructure;

internal static class PersistenceTestServices
{
    public static ServiceProvider Create(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AmbientTenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<AmbientTenantContext>());
        services.AddDbContext<ConsultoriosDbContext>(options => options.UseNpgsql(connectionString));
        return services.BuildServiceProvider();
    }
}
