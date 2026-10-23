using Consultorios.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Consultorios.Api.IntegrationTests;

public class SolutionSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SolutionSmokeTests(WebApplicationFactory<Program> factory)
    {
        DotEnv.LoadFromAncestorDirectory(AppContext.BaseDirectory);
        DotEnv.LoadFromAncestorDirectory(Directory.GetCurrentDirectory());

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("DisableHttpsRedirection", "true");
            builder.UseSetting("SkipSeed", "true");
            builder.UseSetting(
                "ConnectionStrings:Consultorios",
                "Host=127.0.0.1;Port=1;Database=consultorios_smoke;Username=consultorios;Password=smoke");
        });
    }

    [Fact]
    public async Task Liveness_health_endpoint_returns_success()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
