using Consultorios.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Consultorios.Infrastructure.Persistence;

public sealed class ConsultoriosDbContextFactory : IDesignTimeDbContextFactory<ConsultoriosDbContext>
{
    public ConsultoriosDbContext CreateDbContext(string[] args)
    {
        DotEnv.LoadFromAncestorDirectory(Directory.GetCurrentDirectory());
        DotEnv.LoadFromAncestorDirectory(AppContext.BaseDirectory);

        var connection = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"))
            ? "Host=localhost;Port=5432;Database=consultorios;Username=consultorios;Password=design-time"
            : DotEnv.BuildLocalPostgresConnectionString();

        var options = new DbContextOptionsBuilder<ConsultoriosDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new ConsultoriosDbContext(options, new AmbientTenantContext());
    }
}
