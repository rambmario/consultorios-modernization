using Consultorios.Application.Abstractions;
using Consultorios.Application.Common;
using Consultorios.Domain.Common;
using Consultorios.Domain.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Application.Scheduling;

public sealed class AppointmentService
{
    private readonly IConsultoriosDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly TimeProvider _time;

    public AppointmentService(IConsultoriosDbContext db, ITenantContext tenant, TimeProvider time)
    {
        _db = db;
        _tenant = tenant;
        _time = time;
    }

    public async Task<PagedResult<AppointmentResponse>> ListAsync(
        DateTimeOffset? from,
        DateTimeOffset? to,
        Guid? professionalId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (normalizedPage, normalizedSize) = Paging.Normalize(page, pageSize);
        var query = _db.Appointments.AsNoTracking();

        if (from is { } start)
        {
            query = query.Where(a => a.EndsAt > start);
        }

        if (to is { } end)
        {
            query = query.Where(a => a.StartsAt < end);
        }

        if (professionalId is { } professional && professional != Guid.Empty)
        {
            query = query.Where(a => a.ProfessionalId == professional);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.StartsAt)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentResponse>(
            items.Select(ToResponse).ToList(),
            normalizedPage,
            normalizedSize,
            total);
    }

    public async Task<AppointmentResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return appointment is null ? null : ToResponse(appointment);
    }

    public async Task<AppointmentResponse> ScheduleAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var patientExists = await _db.Patients.AnyAsync(p => p.Id == request.PatientId, cancellationToken);
        if (!patientExists)
        {
            throw new DomainException("Patient was not found.");
        }

        var existing = await _db.Appointments
            .Where(a => a.ProfessionalId == request.ProfessionalId)
            .ToListAsync(cancellationToken);

        var appointment = Appointment.Schedule(
            _tenant.TenantId,
            request.PatientId,
            request.ProfessionalId,
            request.StartsAt,
            request.EndsAt,
            existing,
            _time);

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync(cancellationToken);
        return ToResponse(appointment);
    }

    public Task<AppointmentResponse?> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
        => TransitionAsync(id, a => a.Confirm(), cancellationToken);

    public Task<AppointmentResponse?> CancelAsync(Guid id, CancellationToken cancellationToken = default)
        => TransitionAsync(id, a => a.Cancel(), cancellationToken);

    public Task<AppointmentResponse?> MarkAttendedAsync(Guid id, CancellationToken cancellationToken = default)
        => TransitionAsync(id, a => a.MarkAttended(), cancellationToken);

    public Task<AppointmentResponse?> MarkNoShowAsync(Guid id, CancellationToken cancellationToken = default)
        => TransitionAsync(id, a => a.MarkNoShow(), cancellationToken);

    private async Task<AppointmentResponse?> TransitionAsync(
        Guid id,
        Action<Appointment> transition,
        CancellationToken cancellationToken)
    {
        var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        transition(appointment);
        await _db.SaveChangesAsync(cancellationToken);
        return ToResponse(appointment);
    }

    private static AppointmentResponse ToResponse(Appointment appointment)
        => new(
            appointment.Id,
            appointment.PatientId,
            appointment.ProfessionalId,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.Status);
}
