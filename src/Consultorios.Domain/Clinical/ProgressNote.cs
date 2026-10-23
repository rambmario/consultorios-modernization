using Consultorios.Domain.Common;

namespace Consultorios.Domain.Clinical;

public sealed class ProgressNote : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public Guid ProfessionalId { get; private set; }
    public DateTimeOffset SignedAt { get; private set; }
    public ProgressNoteStatus Status { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByProfessionalId { get; private set; }
    public string? CancellationReason { get; private set; }

    public DateTimeOffset? SupersededAt { get; private set; }
    public Guid? SupersededByProfessionalId { get; private set; }
    public Guid? CorrectsNoteId { get; private set; }

    private ProgressNote()
    {
    }

    public static ProgressNote Sign(
        Guid tenantId,
        Guid patientId,
        Guid professionalId,
        string body,
        TimeProvider time,
        Guid? correctsNoteId = null)
    {
        Guard.AgainstEmptyTenant(tenantId);
        ArgumentNullException.ThrowIfNull(time);
        Guard.AgainstEmpty(patientId, "PatientId");
        Guard.AgainstEmpty(professionalId, "ProfessionalId");

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new DomainException("Progress note body is required.");
        }

        return new ProgressNote
        {
            TenantId = tenantId,
            PatientId = patientId,
            ProfessionalId = professionalId,
            Body = body.Trim(),
            SignedAt = time.GetUtcNow(),
            Status = ProgressNoteStatus.Active,
            CorrectsNoteId = correctsNoteId
        };
    }

    public void Cancel(Guid professionalId, string reason, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        Guard.AgainstEmpty(professionalId, "ProfessionalId");
        EnsureActive();
        EnsureSigner(professionalId);

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("Cancellation reason is required.");
        }

        Status = ProgressNoteStatus.Cancelled;
        CancelledAt = time.GetUtcNow();
        CancelledByProfessionalId = professionalId;
        CancellationReason = reason.Trim();
    }

    public ProgressNote Correct(Guid professionalId, string newBody, TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);
        Guard.AgainstEmpty(professionalId, "ProfessionalId");
        EnsureActive();
        EnsureSigner(professionalId);

        if (string.IsNullOrWhiteSpace(newBody))
        {
            throw new DomainException("Corrected progress note body is required.");
        }

        Status = ProgressNoteStatus.Superseded;
        SupersededAt = time.GetUtcNow();
        SupersededByProfessionalId = professionalId;

        return Sign(TenantId, PatientId, professionalId, newBody, time, correctsNoteId: Id);
    }

    private void EnsureActive()
    {
        if (Status != ProgressNoteStatus.Active)
        {
            throw new DomainException("A signed progress note can only be cancelled or corrected while it is active.");
        }
    }

    private void EnsureSigner(Guid professionalId)
    {
        if (professionalId != ProfessionalId)
        {
            throw new DomainException("Only the signing professional can cancel or correct a progress note.");
        }
    }
}
