using Consultorios.Domain.Common;

namespace Consultorios.Domain.Clinical;

public sealed class PatientDiagnosis : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DiagnosisId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }

    public bool IsCurrent => ValidTo is null;

    private PatientDiagnosis()
    {
    }

    public static PatientDiagnosis Open(
        Guid tenantId,
        Guid patientId,
        Guid diagnosisId,
        Guid professionalId,
        IReadOnlyCollection<PatientDiagnosis> existingForPatient,
        TimeProvider time,
        DateTimeOffset? validFrom = null)
    {
        Guard.AgainstEmptyTenant(tenantId);
        ArgumentNullException.ThrowIfNull(existingForPatient);
        ArgumentNullException.ThrowIfNull(time);
        Guard.AgainstEmpty(patientId, "PatientId");
        Guard.AgainstEmpty(diagnosisId, "DiagnosisId");
        Guard.AgainstEmpty(professionalId, "ProfessionalId");

        var now = time.GetUtcNow();
        var from = validFrom ?? now;
        if (from > now)
        {
            throw new DomainException("Diagnosis validity cannot start in the future.");
        }

        if (existingForPatient.Any(item =>
                item.PatientId == patientId
                && item.DiagnosisId == diagnosisId
                && item.IsCurrent))
        {
            throw new DomainException("This diagnosis is already current for the patient.");
        }

        return new PatientDiagnosis
        {
            TenantId = tenantId,
            PatientId = patientId,
            DiagnosisId = diagnosisId,
            ProfessionalId = professionalId,
            RecordedAt = now,
            ValidFrom = from
        };
    }

    public void Close(TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);

        if (!IsCurrent)
        {
            throw new DomainException("Diagnosis validity is already closed.");
        }

        var now = time.GetUtcNow();
        if (now < ValidFrom)
        {
            throw new DomainException("Cannot close a diagnosis before it became valid.");
        }

        ValidTo = now;
    }

}
