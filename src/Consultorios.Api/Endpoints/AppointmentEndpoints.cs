using Consultorios.Api.Filters;
using Consultorios.Application.Auth;
using Consultorios.Application.Scheduling;

namespace Consultorios.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static RouteGroupBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments")
            .WithTags("Appointments")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.Profesional, AppRoles.Recepcion));

        group.MapGet("/", async (
            AppointmentService appointments,
            DateTimeOffset? from,
            DateTimeOffset? to,
            Guid? professionalId,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default) =>
            Results.Ok(await appointments.ListAsync(from, to, professionalId, page, pageSize, ct)));

        group.MapGet("/{id:guid}", async (Guid id, AppointmentService appointments, CancellationToken ct) =>
        {
            var appointment = await appointments.GetAsync(id, ct);
            return appointment is null ? Results.NotFound() : Results.Ok(appointment);
        });

        group.MapPost("/", async (CreateAppointmentRequest request, AppointmentService appointments, CancellationToken ct) =>
            {
                var created = await appointments.ScheduleAsync(request, ct);
                return Results.Created($"/api/appointments/{created.Id}", created);
            })
            .AddEndpointFilter<ValidationFilter<CreateAppointmentRequest>>();

        group.MapPost("/{id:guid}/confirm", async (Guid id, AppointmentService appointments, CancellationToken ct) =>
        {
            var updated = await appointments.ConfirmAsync(id, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapPost("/{id:guid}/cancel", async (Guid id, AppointmentService appointments, CancellationToken ct) =>
        {
            var updated = await appointments.CancelAsync(id, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapPost("/{id:guid}/attend", async (Guid id, AppointmentService appointments, CancellationToken ct) =>
        {
            var updated = await appointments.MarkAttendedAsync(id, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapPost("/{id:guid}/no-show", async (Guid id, AppointmentService appointments, CancellationToken ct) =>
        {
            var updated = await appointments.MarkNoShowAsync(id, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        return group;
    }
}
