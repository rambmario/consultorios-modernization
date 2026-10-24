using System.Threading.RateLimiting;
using Consultorios.Api.Endpoints;
using Consultorios.Api.Middleware;
using Consultorios.Application;
using Consultorios.Infrastructure;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

DotEnv.LoadFromAncestorDirectory(Directory.GetCurrentDirectory());
DotEnv.LoadFromAncestorDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ConsultoriosDbContext>("database", tags: ["ready"]);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth-login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "FrontendDev",
            policy => policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue("EnableOpenApi", false))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("FrontendDev");
}

var seedEnabled = app.Configuration.GetValue("Seed:Enabled", false)
    || (app.Environment.IsDevelopment() && !app.Configuration.GetValue("SkipSeed", false));

if (seedEnabled)
{
    await DemoDataSeeder.SeedAsync(app.Services);
    await DemoUserSeeder.SeedAsync(app.Services);
}

if (!app.Configuration.GetValue("DisableHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<TenantContextMiddleware>();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => !check.Tags.Contains("ready")
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
}).AllowAnonymous();

if (app.Configuration.GetValue("EnableAuthProbe", false))
{
    app.MapGet("/api/__probe/auth-required", () => Results.Ok(new { ok = true }));
}

app.MapAuthEndpoints();
app.MapMeEndpoint();
app.MapPatientEndpoints();
app.MapAppointmentEndpoints();
app.MapCatalogEndpoints();
app.MapClinicalEndpoints();

app.Run();

public partial class Program;
