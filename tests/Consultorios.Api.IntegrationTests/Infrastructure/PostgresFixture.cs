using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Consultorios.Api.IntegrationTests.Infrastructure;

public sealed class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;

    public string ConnectionString =>
        _postgres?.GetConnectionString()
        ?? throw new InvalidOperationException("PostgreSQL container was not started.");

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("consultorios_tests")
            .WithUsername("consultorios")
            .WithPassword("consultorios-test")
            .Build();

        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<ConsultoriosDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new ConsultoriosDbContext(options, new AmbientTenantContext());
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
        {
            await _postgres.DisposeAsync();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
