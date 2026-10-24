using Consultorios.Api.Filters;
using Consultorios.Application.Abstractions;
using Consultorios.Application.Auth;
using Consultorios.Application.Catalog;

namespace Consultorios.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var healthPlans = app.MapGroup("/api/health-plans")
            .WithTags("Catalog")
            .RequireAuthorization(policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.Profesional, AppRoles.Recepcion));

        healthPlans.MapGet("/", async (CatalogService catalog, CancellationToken ct) =>
            Results.Ok(await catalog.ListHealthPlansAsync(ct)));

        healthPlans.MapPost("/", async (CreateHealthPlanRequest request, CatalogService catalog, CancellationToken ct) =>
            {
                var created = await catalog.CreateHealthPlanAsync(request, ct);
                return Results.Created($"/api/health-plans/{created.Id}", created);
            })
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Admin))
            .AddEndpointFilter<ValidationFilter<CreateHealthPlanRequest>>();

        var diagnoses = app.MapGroup("/api/diagnoses")
            .WithTags("Catalog")
            .RequireAuthorization(policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.Profesional, AppRoles.Recepcion));

        diagnoses.MapGet("/", async (CatalogService catalog, CancellationToken ct) =>
            Results.Ok(await catalog.ListDiagnosesAsync(ct)));

        diagnoses.MapPost("/", async (CreateDiagnosisRequest request, CatalogService catalog, CancellationToken ct) =>
            {
                var created = await catalog.CreateDiagnosisAsync(request, ct);
                return Results.Created($"/api/diagnoses/{created.Id}", created);
            })
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Admin))
            .AddEndpointFilter<ValidationFilter<CreateDiagnosisRequest>>();

        app.MapGet("/api/professionals", async (IUserDirectory directory, CancellationToken ct) =>
                Results.Ok(await directory.ListProfessionalsAsync(ct)))
            .WithTags("Catalog")
            .RequireAuthorization(policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.Profesional, AppRoles.Recepcion));

        return app;
    }
}
