using Consultorios.Application.Abstractions;
using Consultorios.Application.Common;
using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Application.Patients;

public sealed class PatientService
{
    private readonly IConsultoriosDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly TimeProvider _time;

    public PatientService(IConsultoriosDbContext db, ITenantContext tenant, TimeProvider time)
    {
        _db = db;
        _tenant = tenant;
        _time = time;
    }

    public async Task<PagedResult<PatientResponse>> ListAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (normalizedPage, normalizedSize) = Paging.Normalize(page, pageSize);
        var query = _db.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.LastName.Contains(term)
                || p.FirstName.Contains(term)
                || p.NationalId.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var patients = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync(cancellationToken);

        var responses = new List<PatientResponse>(patients.Count);
        foreach (var patient in patients)
        {
            responses.Add(await ToResponseAsync(patient, cancellationToken));
        }

        return new PagedResult<PatientResponse>(responses, normalizedPage, normalizedSize, total);
    }

    public async Task<PatientResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return patient is null ? null : await ToResponseAsync(patient, cancellationToken);
    }

    public async Task<PatientResponse> CreateAsync(
        CreatePatientRequest request,
        CancellationToken cancellationToken = default)
    {
        var patient = Patient.Create(
            _tenant.TenantId,
            request.LastName,
            request.FirstName,
            request.NationalId,
            request.Phone,
            ToAddress(request.Address),
            request.FirstConsultationAt);

        _db.Patients.Add(patient);
        _db.Anamneses.Add(Anamnesis.CreateEmpty(_tenant.TenantId, patient.Id, _time));
        await UpsertCoverageAsync(patient.Id, request.HealthPlanId, request.AffiliationNumber, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return await ToResponseAsync(patient, cancellationToken);
    }

    public async Task<PatientResponse?> UpdateAsync(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken = default)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (patient is null)
        {
            return null;
        }

        patient.UpdateDemographics(
            request.LastName,
            request.FirstName,
            request.NationalId,
            request.Phone,
            ToAddress(request.Address));

        await UpsertCoverageAsync(patient.Id, request.HealthPlanId, request.AffiliationNumber, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return await ToResponseAsync(patient, cancellationToken);
    }

    private async Task UpsertCoverageAsync(
        Guid patientId,
        Guid? healthPlanId,
        string? affiliationNumber,
        CancellationToken cancellationToken)
    {
        if (healthPlanId is null || string.IsNullOrWhiteSpace(affiliationNumber))
        {
            return;
        }

        var existing = await _db.PatientCoverages.FirstOrDefaultAsync(c => c.PatientId == patientId, cancellationToken);
        if (existing is null)
        {
            _db.PatientCoverages.Add(PatientCoverage.Create(
                _tenant.TenantId,
                patientId,
                healthPlanId.Value,
                affiliationNumber));
            return;
        }

        existing.Change(healthPlanId.Value, affiliationNumber);
    }

    private async Task<PatientResponse> ToResponseAsync(Patient patient, CancellationToken cancellationToken)
    {
        var coverage = await _db.PatientCoverages
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.PatientId == patient.Id, cancellationToken);

        CoverageDto? coverageDto = null;
        if (coverage is not null)
        {
            var plan = await _db.HealthPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == coverage.HealthPlanId, cancellationToken);
            coverageDto = new CoverageDto(
                coverage.HealthPlanId,
                plan?.Name ?? string.Empty,
                coverage.AffiliationNumber);
        }

        return new PatientResponse(
            patient.Id,
            patient.LastName,
            patient.FirstName,
            patient.NationalId,
            patient.Phone,
            new AddressDto(
                patient.Address.Street,
                patient.Address.Number,
                patient.Address.Apartment,
                patient.Address.City,
                patient.Address.Province,
                patient.Address.PostalCode),
            patient.FirstConsultationAt,
            coverageDto);
    }

    private static Address? ToAddress(AddressDto? dto)
        => dto is null
            ? null
            : new Address(dto.Street, dto.Number, dto.Apartment, dto.City, dto.Province, dto.PostalCode);
}
