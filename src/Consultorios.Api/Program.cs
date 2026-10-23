using Consultorios.Application;
using Consultorios.Infrastructure;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

DotEnv.LoadFromAncestorDirectory(Directory.GetCurrentDirectory());
DotEnv.LoadFromAncestorDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ConsultoriosDbContext>("database", tags: ["ready"]);

var app = builder.Build();

var seedEnabled = app.Configuration.GetValue("Seed:Enabled", false)
    || (app.Environment.IsDevelopment() && !app.Configuration.GetValue("SkipSeed", false));

if (seedEnabled)
{
    await DemoDataSeeder.SeedAsync(app.Services);
}

if (!app.Configuration.GetValue("DisableHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => !check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program;
