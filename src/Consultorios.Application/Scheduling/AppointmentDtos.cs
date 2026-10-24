using Consultorios.Domain.Scheduling;

namespace Consultorios.Application.Scheduling;

public sealed record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    Guid ProfessionalId,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    AppointmentStatus Status);

public sealed record CreateAppointmentRequest(
    Guid PatientId,
    Guid ProfessionalId,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt);
