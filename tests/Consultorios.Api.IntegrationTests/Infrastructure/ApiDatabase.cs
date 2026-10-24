using Npgsql;

namespace Consultorios.Api.IntegrationTests.Infrastructure;

internal static class ApiDatabase
{
    public const string Name = "consultorios_api_tests";

    public static async Task EnsureCreatedAsync(string fixtureConnectionString)
    {
        var admin = new NpgsqlConnectionStringBuilder(fixtureConnectionString)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(admin.ConnectionString);
        await connection.OpenAsync();
        await using var existsCommand = new NpgsqlCommand(
            $"SELECT 1 FROM pg_database WHERE datname = '{Name}'",
            connection);
        if (await existsCommand.ExecuteScalarAsync() is not null)
        {
            return;
        }

        await using var create = new NpgsqlCommand($"CREATE DATABASE {Name}", connection);
        await create.ExecuteNonQueryAsync();
    }
}
