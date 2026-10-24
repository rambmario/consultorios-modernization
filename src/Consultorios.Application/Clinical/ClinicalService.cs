using Consultorios.Application.Abstractions;
using Consultorios.Application.Patients;
using Consultorios.Domain.Clinical;
using Consultorios.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Application.Clinical;

public sealed class ClinicalService
{
    private readonly IConsultoriosDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ICurrentUser _user;
    private readonly TimeProvider _time;
    private readonly PatientService _patients;

    public ClinicalService(
        IConsultoriosDbContext db,
        ITenantContext tenant,
        ICurrentUser user,
        TimeProvider time,
        PatientService patients)
    {
        _db = db;
        _tenant = tenant;
        _user = user;
        _time = time;
        _patients = patients;
    }

    public async Task<ClinicalRecordResponse?> GetRecordAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetAsync(patientId, cancellationToken);
        if (patient is null)
        {
            return null;
        }

        var anamnesis = await EnsureAnamnesisAsync(patientId, cancellationToken);
        var notes = await ListNotesAsync(patientId, cancellationToken) ?? [];
        var diagnoses = await ListPatientDiagnosesAsync(patientId, cancellationToken) ?? [];

        await AuditAsync(patientId, ClinicalAccessAction.Viewed, "ClinicalRecord", patientId, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new ClinicalRecordResponse(patient, ToAnamnesisResponse(anamnesis), notes, diagnoses);
    }

    public async Task<AnamnesisResponse?> GetAnamnesisAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var anamnesis = await EnsureAnamnesisAsync(patientId, cancellationToken);
        return ToAnamnesisResponse(anamnesis);
    }

    public async Task<AnamnesisResponse?> ReplaceAnamnesisAsync(
        Guid patientId,
        ReplaceAnamnesisRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var anamnesis = await EnsureAnamnesisAsync(patientId, cancellationToken);
        anamnesis.ReplaceSections(
            request.ChiefComplaint,
            request.CurrentIllnessHistory,
            request.PersonalPathologicalHistory,
            request.FamilyHistory,
            request.PhysicalExamination,
            request.HabitHistory,
            _time);

        await AuditAsync(patientId, ClinicalAccessAction.Updated, "Anamnesis", anamnesis.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return ToAnamnesisResponse(anamnesis);
    }

    public async Task<IReadOnlyList<ProgressNoteResponse>?> ListNotesAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var notes = await _db.ProgressNotes
            .AsNoTracking()
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.SignedAt)
            .ToListAsync(cancellationToken);

        return notes.Select(ToNoteResponse).ToList();
    }

    public async Task<ProgressNoteResponse?> SignNoteAsync(
        Guid patientId,
        SignProgressNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var note = ProgressNote.Sign(
            _tenant.TenantId,
            patientId,
            _user.UserId,
            request.Body,
            _time);

        _db.ProgressNotes.Add(note);
        await AuditAsync(patientId, ClinicalAccessAction.Created, "ProgressNote", note.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return ToNoteResponse(note);
    }

    public async Task<ProgressNoteResponse?> CancelNoteAsync(
        Guid noteId,
        CancelProgressNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var note = await _db.ProgressNotes.FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        if (note is null)
        {
            return null;
        }

        note.Cancel(_user.UserId, request.Reason, _time);
        await AuditAsync(note.PatientId, ClinicalAccessAction.Cancelled, "ProgressNote", note.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return ToNoteResponse(note);
    }

    public async Task<ProgressNoteResponse?> CorrectNoteAsync(
        Guid noteId,
        CorrectProgressNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var note = await _db.ProgressNotes.FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        if (note is null)
        {
            return null;
        }

        var replacement = note.Correct(_user.UserId, request.Body, _time);
        _db.ProgressNotes.Add(replacement);
        await AuditAsync(note.PatientId, ClinicalAccessAction.Corrected, "ProgressNote", replacement.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return ToNoteResponse(replacement);
    }

    public async Task<IReadOnlyList<PatientDiagnosisResponse>?> ListPatientDiagnosesAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var items = await _db.PatientDiagnoses
            .AsNoTracking()
            .Where(d => d.PatientId == patientId)
            .OrderByDescending(d => d.ValidFrom)
            .ToListAsync(cancellationToken);

        var names = await _db.Diagnoses
            .AsNoTracking()
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        return items
            .Select(item => new PatientDiagnosisResponse(
                item.Id,
                item.PatientId,
                item.DiagnosisId,
                names.GetValueOrDefault(item.DiagnosisId, string.Empty),
                item.ProfessionalId,
                item.RecordedAt,
                item.ValidFrom,
                item.ValidTo,
                item.IsCurrent))
            .ToList();
    }

    public async Task<PatientDiagnosisResponse?> OpenDiagnosisAsync(
        Guid patientId,
        OpenPatientDiagnosisRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await PatientExistsAsync(patientId, cancellationToken))
        {
            return null;
        }

        var diagnosis = await _db.Diagnoses.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DiagnosisId, cancellationToken);
        if (diagnosis is null)
        {
            return null;
        }

        var existing = await _db.PatientDiagnoses
            .Where(d => d.PatientId == patientId)
            .ToListAsync(cancellationToken);

        var opened = PatientDiagnosis.Open(
            _tenant.TenantId,
            patientId,
            request.DiagnosisId,
            _user.UserId,
            existing,
            _time,
            request.ValidFrom);

        _db.PatientDiagnoses.Add(opened);
        await AuditAsync(patientId, ClinicalAccessAction.Created, "PatientDiagnosis", opened.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new PatientDiagnosisResponse(
            opened.Id,
            opened.PatientId,
            opened.DiagnosisId,
            diagnosis.Name,
            opened.ProfessionalId,
            opened.RecordedAt,
            opened.ValidFrom,
            opened.ValidTo,
            opened.IsCurrent);
    }

    public async Task<PatientDiagnosisResponse?> CloseDiagnosisAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _db.PatientDiagnoses.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        item.Close(_time);
        await AuditAsync(item.PatientId, ClinicalAccessAction.Updated, "PatientDiagnosis", item.Id, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var name = await _db.Diagnoses.AsNoTracking()
            .Where(d => d.Id == item.DiagnosisId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new PatientDiagnosisResponse(
            item.Id,
            item.PatientId,
            item.DiagnosisId,
            name ?? string.Empty,
            item.ProfessionalId,
            item.RecordedAt,
            item.ValidFrom,
            item.ValidTo,
            item.IsCurrent);
    }

    public async Task<IReadOnlyList<ClinicalAuditResponse>> ListAuditsAsync(
        Guid? patientId,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ClinicalAccessAudits.AsNoTracking();
        if (patientId is { } id && id != Guid.Empty)
        {
            query = query.Where(a => a.PatientId == id);
        }

        return await query
            .OrderByDescending(a => a.OccurredAt)
            .Take(PagingSafeLimit)
            .Select(a => new ClinicalAuditResponse(
                a.Id,
                a.ActorUserId,
                a.PatientId,
                a.Action,
                a.ResourceType,
                a.ResourceId,
                a.OccurredAt))
            .ToListAsync(cancellationToken);
    }

    private const int PagingSafeLimit = 200;

    private async Task<Anamnesis> EnsureAnamnesisAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var existing = await _db.Anamneses.FirstOrDefaultAsync(a => a.PatientId == patientId, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var created = Anamnesis.CreateEmpty(_tenant.TenantId, patientId, _time);
        _db.Anamneses.Add(created);
        await _db.SaveChangesAsync(cancellationToken);
        return created;
    }

    private Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken)
        => _db.Patients.AsNoTracking().AnyAsync(p => p.Id == patientId, cancellationToken);

    private async Task AuditAsync(
        Guid patientId,
        ClinicalAccessAction action,
        string resourceType,
        Guid resourceId,
        CancellationToken cancellationToken)
    {
        _db.ClinicalAccessAudits.Add(ClinicalAccessAudit.Record(
            _tenant.TenantId,
            _user.UserId,
            patientId,
            action,
            resourceType,
            resourceId,
            _time));
        await Task.CompletedTask;
    }

    private static AnamnesisResponse ToAnamnesisResponse(Anamnesis anamnesis)
        => new(
            anamnesis.Id,
            anamnesis.PatientId,
            anamnesis.ChiefComplaint,
            anamnesis.CurrentIllnessHistory,
            anamnesis.PersonalPathologicalHistory,
            anamnesis.FamilyHistory,
            anamnesis.PhysicalExamination,
            anamnesis.HabitHistory,
            anamnesis.UpdatedAt);

    private static ProgressNoteResponse ToNoteResponse(ProgressNote note)
        => new(
            note.Id,
            note.PatientId,
            note.ProfessionalId,
            note.Body,
            note.SignedAt,
            note.Status,
            note.CorrectsNoteId,
            note.CancellationReason);
}
