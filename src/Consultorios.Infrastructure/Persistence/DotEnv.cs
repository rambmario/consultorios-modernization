namespace Consultorios.Infrastructure.Persistence;

/// <summary>
/// Minimal .env loader for local development (docker-compose and API/tests).
/// </summary>
public static class DotEnv
{
    public static void LoadFromAncestorDirectory(string startDirectory, string fileName = ".env")
    {
        var dir = new DirectoryInfo(startDirectory);
        while (dir is not null)
        {
            var path = Path.Combine(dir.FullName, fileName);
            if (File.Exists(path))
            {
                Load(path);
                return;
            }

            dir = dir.Parent;
        }
    }

    public static void Load(string path)
    {
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || !line.Contains('='))
            {
                continue;
            }

            var separator = line.IndexOf('=');
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"').Trim('\'');
            if (key.Length == 0)
            {
                continue;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    public static string RequirePostgresPassword()
    {
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "POSTGRES_PASSWORD is not set. Copy .env.example to .env and set the password.");
        }

        return password;
    }

    public static string BuildLocalPostgresConnectionString()
    {
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "consultorios";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "consultorios";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var password = RequirePostgresPassword();
        return $"Host=localhost;Port={port};Database={database};Username={user};Password={password}";
    }
}
