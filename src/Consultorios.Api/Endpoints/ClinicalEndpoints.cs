using Consultorios.Api.Filters;
using Consultorios.Application.Auth;
using Consultorios.Application.Clinical;

namespace Consultorios.Api.Endpoints;

public static class ClinicalEndpoints
{
    public static IEndpointRouteBuilder MapClinicalEndpoints(this IEndpointRouteBuilder app)
    {
        var clinical = app.MapGroup("/api/patients/{patientId:guid}")
            .WithTags("Clinical")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Profesional));

        clinical.MapGet("/clinical-record", async (Guid patientId, ClinicalService clinicalService, CancellationToken ct) =>
        {
            var record = await clinicalService.GetRecordAsync(patientId, ct);
            return record is null ? Results.NotFound() : Results.Ok(record);
        });

        clinical.MapGet("/anamnesis", async (Guid patientId, ClinicalService clinicalService, CancellationToken ct) =>
        {
            var anamnesis = await clinicalService.GetAnamnesisAsync(patientId, ct);
            return anamnesis is null ? Results.NotFound() : Results.Ok(anamnesis);
        });

        clinical.MapPut("/anamnesis", async (
            Guid patientId,
            ReplaceAnamnesisRequest request,
            ClinicalService clinicalService,
            CancellationToken ct) =>
        {
            var updated = await clinicalService.ReplaceAnamnesisAsync(patientId, request, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        clinical.MapGet("/progress-notes", async (Guid patientId, ClinicalService clinicalService, CancellationToken ct) =>
        {
            var notes = await clinicalService.ListNotesAsync(patientId, ct);
            return notes is null ? Results.NotFound() : Results.Ok(notes);
        });

        clinical.MapPost("/progress-notes", async (
                Guid patientId,
                SignProgressNoteRequest request,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            {
                var created = await clinicalService.SignNoteAsync(patientId, request, ct);
                return created is null
                    ? Results.NotFound()
                    : Results.Created($"/api/patients/{patientId}/progress-notes", created);
            })
            .AddEndpointFilter<ValidationFilter<SignProgressNoteRequest>>();

        clinical.MapGet("/diagnoses", async (Guid patientId, ClinicalService clinicalService, CancellationToken ct) =>
        {
            var diagnoses = await clinicalService.ListPatientDiagnosesAsync(patientId, ct);
            return diagnoses is null ? Results.NotFound() : Results.Ok(diagnoses);
        });

        clinical.MapPost("/diagnoses", async (
                Guid patientId,
                OpenPatientDiagnosisRequest request,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            {
                var created = await clinicalService.OpenDiagnosisAsync(patientId, request, ct);
                return created is null ? Results.NotFound() : Results.Created($"/api/patient-diagnoses/{created.Id}", created);
            })
            .AddEndpointFilter<ValidationFilter<OpenPatientDiagnosisRequest>>();

        app.MapPost("/api/progress-notes/{id:guid}/cancel", async (
                Guid id,
                CancelProgressNoteRequest request,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            {
                var updated = await clinicalService.CancelNoteAsync(id, request, ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .WithTags("Clinical")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Profesional))
            .AddEndpointFilter<ValidationFilter<CancelProgressNoteRequest>>();

        app.MapPost("/api/progress-notes/{id:guid}/correct", async (
                Guid id,
                CorrectProgressNoteRequest request,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            {
                var updated = await clinicalService.CorrectNoteAsync(id, request, ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .WithTags("Clinical")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Profesional))
            .AddEndpointFilter<ValidationFilter<CorrectProgressNoteRequest>>();

        app.MapPost("/api/patient-diagnoses/{id:guid}/close", async (
                Guid id,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            {
                var updated = await clinicalService.CloseDiagnosisAsync(id, ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .WithTags("Clinical")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Profesional));

        app.MapGet("/api/clinical-audits", async (
                Guid? patientId,
                ClinicalService clinicalService,
                CancellationToken ct) =>
            Results.Ok(await clinicalService.ListAuditsAsync(patientId, ct)))
            .WithTags("Clinical")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Admin, AppRoles.Profesional));

        return app;
    }
}
