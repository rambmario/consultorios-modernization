namespace Consultorios.Domain.Scheduling;

/// <summary>
/// Integer values are stored in PostgreSQL and referenced by
/// <c>EX_Appointments_ProfessionalOverlap</c> (<c>WHERE Status IN (0, 1)</c>).
/// Changing <see cref="Scheduled"/> or <see cref="Confirmed"/> breaks that constraint.
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    Attended = 2,
    Cancelled = 3,
    NoShow = 4
}
