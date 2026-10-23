using Consultorios.Domain.Common;

namespace Consultorios.Domain.Patients;

public sealed class Anamnesis : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public string ChiefComplaint { get; private set; } = string.Empty;
    public string CurrentIllnessHistory { get; private set; } = string.Empty;
    public string PersonalPathologicalHistory { get; private set; } = string.Empty;
    public string FamilyHistory { get; private set; } = string.Empty;
    public string PhysicalExamination { get; private set; } = string.Empty;
    public string HabitHistory { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; private set; }

    private Anamnesis()
    {
    }

    public static Anamnesis CreateEmpty(Guid tenantId, Guid patientId, TimeProvider time)
    {
        Guard.AgainstEmptyTenant(tenantId);
        ArgumentNullException.ThrowIfNull(time);
        EnsurePatient(patientId);

        return new Anamnesis
        {
            TenantId = tenantId,
            PatientId = patientId,
            UpdatedAt = time.GetUtcNow()
        };
    }

    public void ReplaceSections(
        string? chiefComplaint,
        string? currentIllnessHistory,
        string? personalPathologicalHistory,
        string? familyHistory,
        string? physicalExamination,
        string? habitHistory,
        TimeProvider time)
    {
        ArgumentNullException.ThrowIfNull(time);

        ChiefComplaint = Normalize(chiefComplaint);
        CurrentIllnessHistory = Normalize(currentIllnessHistory);
        PersonalPathologicalHistory = Normalize(personalPathologicalHistory);
        FamilyHistory = Normalize(familyHistory);
        PhysicalExamination = Normalize(physicalExamination);
        HabitHistory = Normalize(habitHistory);
        UpdatedAt = time.GetUtcNow();
    }

    public IReadOnlyDictionary<string, string> SectionsByLongName()
        => new Dictionary<string, string>
        {
            [AnamnesisSections.ChiefComplaint] = ChiefComplaint,
            [AnamnesisSections.CurrentIllnessHistory] = CurrentIllnessHistory,
            [AnamnesisSections.PersonalPathologicalHistory] = PersonalPathologicalHistory,
            [AnamnesisSections.FamilyHistory] = FamilyHistory,
            [AnamnesisSections.PhysicalExamination] = PhysicalExamination,
            [AnamnesisSections.HabitHistory] = HabitHistory
        };

    private static void EnsurePatient(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("PatientId is required.");
        }
    }

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}
