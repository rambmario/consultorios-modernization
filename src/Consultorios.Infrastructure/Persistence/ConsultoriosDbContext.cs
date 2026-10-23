using System.Linq.Expressions;
using Consultorios.Application.Abstractions;
using Consultorios.Domain.Clinical;
using Consultorios.Domain.Common;
using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Infrastructure.Persistence;

public sealed class ConsultoriosDbContext : DbContext, IConsultoriosDbContext
{
    private readonly ITenantContext _tenantContext;

    public ConsultoriosDbContext(DbContextOptions<ConsultoriosDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Anamnesis> Anamneses => Set<Anamnesis>();
    public DbSet<HealthPlan> HealthPlans => Set<HealthPlan>();
    public DbSet<PatientCoverage> PatientCoverages => Set<PatientCoverage>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<PatientDiagnosis> PatientDiagnoses => Set<PatientDiagnosis>();
    public DbSet<ProgressNote> ProgressNotes => Set<ProgressNote>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ClinicalAccessAudit> ClinicalAccessAudits => Set<ClinicalAccessAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsultoriosDbContext).Assembly);
        ApplyFailClosedTenantFilters(modelBuilder);
    }

    private void ApplyFailClosedTenantFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(ConsultoriosDbContext)
                .GetMethod(nameof(SetTenantQueryFilter), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [modelBuilder]);
        }
    }

    private void SetTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantOwned
    {
        Expression<Func<TEntity, bool>> filter =
            e => e.TenantId == _tenantContext.CurrentTenantIdOrEmpty;

        modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
    }
}
