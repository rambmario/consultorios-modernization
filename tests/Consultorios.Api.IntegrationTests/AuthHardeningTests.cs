using System.Net;
using System.Net.Http.Json;
using Consultorios.Api.IntegrationTests.Infrastructure;
using Consultorios.Application.Auth;
using Consultorios.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Consultorios.Api.IntegrationTests;

public sealed class AuthHardeningTests
{
    [Fact]
    public async Task Host_fails_to_start_without_jwt_signing_key()
    {
        var previous = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        try
        {
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "short");

            await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("SkipSeed", "true");
                builder.UseSetting("Seed:Enabled", "false");
                builder.UseSetting("Jwt:SigningKey", "short");
                builder.UseSetting(
                    "ConnectionStrings:Consultorios",
                    "Host=127.0.0.1;Port=1;Database=consultorios_auth;Username=consultorios;Password=smoke");
            });

            Exception? caught = null;
            try
            {
                var client = factory.CreateClient();
                await client.GetAsync("/health");
            }
            catch (Exception ex)
            {
                caught = ex;
            }

            caught.Should().NotBeNull("host must fail when Jwt:SigningKey is too short");
            caught!.ToString().Should().Contain("SigningKey");
        }
        finally
        {
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previous);
        }
    }

    [Fact]
    public async Task Probe_endpoint_without_auth_attributes_returns_401()
    {
        TestAuth.ApplyEnvironment();

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("SkipSeed", "true");
            builder.UseSetting("Seed:Enabled", "false");
            builder.UseSetting("EnableAuthProbe", "true");
            builder.UseSetting("DisableHttpsRedirection", "true");
            builder.UseSetting("Jwt:SigningKey", TestAuth.JwtSigningKey);
            builder.UseSetting(
                "ConnectionStrings:Consultorios",
                "Host=127.0.0.1;Port=1;Database=consultorios_probe;Username=consultorios;Password=smoke");
        });

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/__probe/auth-required");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

[Collection(PostgresCollection.Name)]
public sealed class AuthRateLimitTests : IAsyncLifetime
{
    private readonly PostgresFixture _postgres;
    private ConsultoriosApiFactory? _factory;
    private HttpClient? _client;
    private HttpClient Client => _client ?? throw new InvalidOperationException("HTTP client was not initialized.");

    public AuthRateLimitTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    public async Task InitializeAsync()
    {
        await ApiDatabase.EnsureCreatedAsync(_postgres.ConnectionString);
        _factory = new ConsultoriosApiFactory(_postgres);
        _client = _factory.CreateClient();
        await _client.GetAsync("/health");
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }

    [Fact]
    public async Task Login_rate_limit_returns_429()
    {
        HttpResponseMessage? last = null;
        for (var i = 0; i < 8; i++)
        {
            last = await Client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest("nobody@demo.local", "WrongPass123"));
        }

        last!.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
