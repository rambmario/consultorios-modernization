using System.Security.Claims;
using Consultorios.Application.Auth;
using Consultorios.Infrastructure.Identity;
using Consultorios.Infrastructure.Tenancy;

namespace Consultorios.Api.Middleware;

public sealed class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        AmbientTenantContext tenantContext,
        AmbientCurrentUser currentUser)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var rawTenant = context.User.FindFirstValue(TenantClaimTypes.TenantId)
                            ?? context.User.FindFirstValue("tenant_id");

            if (string.IsNullOrWhiteSpace(rawTenant)
                || !Guid.TryParse(rawTenant, out var tenantId)
                || tenantId == Guid.Empty)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    title = "Unauthorized",
                    detail = "Authentication token is missing a valid tenant_id claim.",
                    status = StatusCodes.Status401Unauthorized
                });
                return;
            }

            tenantContext.SetTenant(tenantId);

            var rawUser = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? context.User.FindFirstValue("sub");
            if (Guid.TryParse(rawUser, out var userId) && userId != Guid.Empty)
            {
                currentUser.SetUser(userId);
            }
        }

        await _next(context);
    }
}
