using Consultorios.Application.Patients;
using Consultorios.Domain.Clinical;

namespace Consultorios.Application.Clinical;

public sealed record AnamnesisResponse(
    Guid Id,
    Guid PatientId,
    string ChiefComplaint,
    string CurrentIllnessHistory,
    string PersonalPathologicalHistory,
    string FamilyHistory,
    string PhysicalExamination,
    string HabitHistory,
    DateTimeOffset UpdatedAt);

public sealed record ReplaceAnamnesisRequest(
    string? ChiefComplaint,
    string? CurrentIllnessHistory,
    string? PersonalPathologicalHistory,
    string? FamilyHistory,
    string? PhysicalExamination,
    string? HabitHistory);

public sealed record ProgressNoteResponse(
    Guid Id,
    Guid PatientId,
    Guid ProfessionalId,
    string Body,
    DateTimeOffset SignedAt,
    ProgressNoteStatus Status,
    Guid? CorrectsNoteId,
    string? CancellationReason);

public sealed record SignProgressNoteRequest(string Body);

public sealed record CancelProgressNoteRequest(string Reason);

public sealed record CorrectProgressNoteRequest(string Body);

public sealed record PatientDiagnosisResponse(
    Guid Id,
    Guid PatientId,
    Guid DiagnosisId,
    string DiagnosisName,
    Guid ProfessionalId,
    DateTimeOffset RecordedAt,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo,
    bool IsCurrent);

public sealed record OpenPatientDiagnosisRequest(Guid DiagnosisId, DateTimeOffset? ValidFrom);

public sealed record ClinicalRecordResponse(
    PatientResponse Patient,
    AnamnesisResponse Anamnesis,
    IReadOnlyList<ProgressNoteResponse> ProgressNotes,
    IReadOnlyList<PatientDiagnosisResponse> Diagnoses);

public sealed record ClinicalAuditResponse(
    Guid Id,
    Guid ActorUserId,
    Guid PatientId,
    ClinicalAccessAction Action,
    string ResourceType,
    Guid ResourceId,
    DateTimeOffset OccurredAt);
