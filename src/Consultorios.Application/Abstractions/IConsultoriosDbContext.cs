using Consultorios.Domain.Clinical;
using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Consultorios.Application.Abstractions;

public interface IConsultoriosDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Patient> Patients { get; }
    DbSet<Anamnesis> Anamneses { get; }
    DbSet<HealthPlan> HealthPlans { get; }
    DbSet<PatientCoverage> PatientCoverages { get; }
    DbSet<Diagnosis> Diagnoses { get; }
    DbSet<PatientDiagnosis> PatientDiagnoses { get; }
    DbSet<ProgressNote> ProgressNotes { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<ClinicalAccessAudit> ClinicalAccessAudits { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
