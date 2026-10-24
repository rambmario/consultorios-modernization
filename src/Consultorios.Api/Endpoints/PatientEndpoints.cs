using Consultorios.Api.Filters;
using Consultorios.Application.Auth;
using Consultorios.Application.Patients;

namespace Consultorios.Api.Endpoints;

public static class PatientEndpoints
{
    public static RouteGroupBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients")
            .WithTags("Patients")
            .RequireAuthorization(policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.Profesional, AppRoles.Recepcion));

        group.MapGet("/", async (
            PatientService patients,
            string? search,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default) =>
            Results.Ok(await patients.ListAsync(search, page, pageSize, ct)));

        group.MapGet("/{id:guid}", async (Guid id, PatientService patients, CancellationToken ct) =>
        {
            var patient = await patients.GetAsync(id, ct);
            return patient is null ? Results.NotFound() : Results.Ok(patient);
        });

        group.MapPost("/", async (CreatePatientRequest request, PatientService patients, CancellationToken ct) =>
            {
                var created = await patients.CreateAsync(request, ct);
                return Results.Created($"/api/patients/{created.Id}", created);
            })
            .AddEndpointFilter<ValidationFilter<CreatePatientRequest>>();

        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdatePatientRequest request,
                PatientService patients,
                CancellationToken ct) =>
            {
                var updated = await patients.UpdateAsync(id, request, ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .AddEndpointFilter<ValidationFilter<UpdatePatientRequest>>();

        return group;
    }
}
