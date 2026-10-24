using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Consultorios.Api.IntegrationTests.Infrastructure;

public sealed class ConsultoriosApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ConsultoriosApiFactory(PostgresFixture postgres)
    {
        var builder = new NpgsqlConnectionStringBuilder(postgres.ConnectionString)
        {
            Database = "consultorios_api_tests"
        };
        _connectionString = builder.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        TestAuth.ApplyEnvironment();

        builder.UseEnvironment("Development");
        builder.UseSetting("DisableHttpsRedirection", "true");
        builder.UseSetting("SkipSeed", "false");
        builder.UseSetting("Seed:Enabled", "true");
        builder.UseSetting("EnableAuthProbe", "true");
        builder.UseSetting("Jwt:SigningKey", TestAuth.JwtSigningKey);
        builder.UseSetting("ConnectionStrings:Consultorios", _connectionString);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Consultorios"] = _connectionString,
                ["Jwt:SigningKey"] = TestAuth.JwtSigningKey,
                ["SkipSeed"] = "false",
                ["Seed:Enabled"] = "true",
                ["EnableAuthProbe"] = "true",
                ["DisableHttpsRedirection"] = "true"
            });
        });
    }
}
