using System.Security.Claims;
using Consultorios.Api.Filters;
using Consultorios.Application.Auth;
using Microsoft.AspNetCore.RateLimiting;

namespace Consultorios.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, IAuthService auth, CancellationToken ct) =>
            {
                var response = await auth.LoginAsync(request, ct);
                return response is null
                    ? Results.Unauthorized()
                    : Results.Ok(response);
            })
            .AllowAnonymous()
            .RequireRateLimiting("auth-login")
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .WithName("Login");

        return group;
    }

    public static IEndpointRouteBuilder MapMeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/me", (ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email");
            var tenantId = user.FindFirstValue(TenantClaimTypes.TenantId);
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            return Results.Ok(new { userId, email, tenantId, roles });
        }).WithTags("Auth");

        return app;
    }
}
