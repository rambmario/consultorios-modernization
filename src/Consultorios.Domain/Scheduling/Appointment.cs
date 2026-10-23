using Consultorios.Domain.Common;
using Consultorios.Domain.Patients;

namespace Consultorios.Domain.Scheduling;

public sealed class Appointment : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public bool OccupiesSlot
        => Status is AppointmentStatus.Scheduled or AppointmentStatus.Confirmed;

    private Appointment()
    {
    }

    public static Appointment Schedule(
        Guid tenantId,
        Guid patientId,
        Guid professionalId,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        IReadOnlyCollection<Appointment> existingForProfessional,
        TimeProvider time)
    {
        Patient.EnsureTenant(tenantId);
        ArgumentNullException.ThrowIfNull(existingForProfessional);
        ArgumentNullException.ThrowIfNull(time);

        if (patientId == Guid.Empty)
        {
            throw new DomainException("PatientId is required.");
        }

        if (professionalId == Guid.Empty)
        {
            throw new DomainException("ProfessionalId is required.");
        }

        if (endsAt <= startsAt)
        {
            throw new DomainException("Appointment end must be after start.");
        }

        var appointment = new Appointment
        {
            TenantId = tenantId,
            PatientId = patientId,
            ProfessionalId = professionalId,
            StartsAt = startsAt,
            EndsAt = endsAt,
            Status = AppointmentStatus.Scheduled,
            CreatedAt = time.GetUtcNow()
        };

        EnsureNoOverlap(existingForProfessional, appointment);
        return appointment;
    }

    public static void EnsureNoOverlap(
        IReadOnlyCollection<Appointment> existingForProfessional,
        Appointment candidate)
    {
        ArgumentNullException.ThrowIfNull(existingForProfessional);
        ArgumentNullException.ThrowIfNull(candidate);

        var overlaps = existingForProfessional.Any(existing =>
            existing.Id != candidate.Id
            && existing.ProfessionalId == candidate.ProfessionalId
            && existing.OccupiesSlot
            && IntervalsOverlap(existing.StartsAt, existing.EndsAt, candidate.StartsAt, candidate.EndsAt));

        if (overlaps)
        {
            throw new DomainException("The professional already has an appointment in that time range.");
        }
    }

    public void Confirm()
        => Transition(AppointmentStatus.Scheduled, AppointmentStatus.Confirmed);

    public void MarkAttended()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Confirmed))
        {
            throw new DomainException($"Cannot mark attended from status {Status}.");
        }

        Status = AppointmentStatus.Attended;
    }

    public void Cancel()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Confirmed))
        {
            throw new DomainException($"Cannot cancel appointment from status {Status}.");
        }

        Status = AppointmentStatus.Cancelled;
    }

    public void MarkNoShow()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Confirmed))
        {
            throw new DomainException($"Cannot mark no-show from status {Status}.");
        }

        Status = AppointmentStatus.NoShow;
    }

    private void Transition(AppointmentStatus from, AppointmentStatus to)
    {
        if (Status != from)
        {
            throw new DomainException($"Cannot move appointment from {Status} to {to}.");
        }

        Status = to;
    }

    private static bool IntervalsOverlap(
        DateTimeOffset startA,
        DateTimeOffset endA,
        DateTimeOffset startB,
        DateTimeOffset endB)
        => startA < endB && startB < endA;
}
