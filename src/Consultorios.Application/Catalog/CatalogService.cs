using Consultorios.Application.Abstractions;
using Consultorios.Domain.Clinical;
using Consultorios.Domain.HealthPlans;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Application.Catalog;

public sealed class CatalogService
{
    private readonly IConsultoriosDbContext _db;
    private readonly ITenantContext _tenant;

    public CatalogService(IConsultoriosDbContext db, ITenantContext tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<IReadOnlyList<HealthPlanResponse>> ListHealthPlansAsync(CancellationToken cancellationToken = default)
        => await _db.HealthPlans
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new HealthPlanResponse(p.Id, p.Name))
            .ToListAsync(cancellationToken);

    public async Task<HealthPlanResponse> CreateHealthPlanAsync(
        CreateHealthPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var plan = HealthPlan.Create(_tenant.TenantId, request.Name);
        _db.HealthPlans.Add(plan);
        await _db.SaveChangesAsync(cancellationToken);
        return new HealthPlanResponse(plan.Id, plan.Name);
    }

    public async Task<IReadOnlyList<DiagnosisCatalogResponse>> ListDiagnosesAsync(
        CancellationToken cancellationToken = default)
        => await _db.Diagnoses
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DiagnosisCatalogResponse(d.Id, d.Name, d.Details))
            .ToListAsync(cancellationToken);

    public async Task<DiagnosisCatalogResponse> CreateDiagnosisAsync(
        CreateDiagnosisRequest request,
        CancellationToken cancellationToken = default)
    {
        var diagnosis = Diagnosis.Create(_tenant.TenantId, request.Name, request.Details);
        _db.Diagnoses.Add(diagnosis);
        await _db.SaveChangesAsync(cancellationToken);
        return new DiagnosisCatalogResponse(diagnosis.Id, diagnosis.Name, diagnosis.Details);
    }
}
