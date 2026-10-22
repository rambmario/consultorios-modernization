using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Consultorios.Api.IntegrationTests;

public class SolutionSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SolutionSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("DisableHttpsRedirection", "true");
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
