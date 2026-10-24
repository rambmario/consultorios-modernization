using Consultorios.Domain.Clinical;
using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tenancy;
using Consultorios.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Consultorios.Infrastructure.Persistence.Seed;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        var time = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoDataSeeder");
        var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();

        await db.Database.MigrateAsync(cancellationToken);

        // System process: no ambient tenant; use IgnoreQueryFilters for any tenant-owned reads.
        tenantContext.Clear();

        if (await db.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Name == "ClinicaA", cancellationToken))
        {
            logger.LogInformation("Demo data already present; skipping seed.");
            return;
        }

        var clinicA = Tenant.Create("ClinicaA");
        var clinicB = Tenant.Create("ClinicaB");
        db.Tenants.AddRange(clinicA, clinicB);
        await db.SaveChangesAsync(cancellationToken);

        await SeedClinicAAsync(db, time, clinicA, cancellationToken);
        await SeedClinicBAsync(db, time, clinicB, cancellationToken);

        logger.LogInformation("Seeded demo tenants '{ClinicA}' and '{ClinicB}'.", clinicA.Name, clinicB.Name);
    }

    private static async Task SeedClinicAAsync(
        ConsultoriosDbContext db,
        TimeProvider time,
        Tenant clinicA,
        CancellationToken cancellationToken)
    {
        var patient = Patient.Create(
            clinicA.Id,
            "Demo",
            "Paciente Demo 1",
            "20000001",
            phone: null,
            address: new Address("Calle Falsa", "123", null, "Ciudad Demo", "Provincia Demo", "1000"));

        var plan = HealthPlan.Create(clinicA.Id, "OS Demo A");
        var coverage = PatientCoverage.Create(clinicA.Id, patient.Id, plan.Id, "AF-DEMO-0001");
        var anamnesis = Anamnesis.CreateEmpty(clinicA.Id, patient.Id, time);
        var diagnosis = Diagnosis.Create(clinicA.Id, "Hipertension esencial (demo)", "Catalogo ficticio");
        var patientDiagnosis = PatientDiagnosis.Open(
            clinicA.Id,
            patient.Id,
            diagnosis.Id,
            DemoIds.ProfesionalA,
            [],
            time);
        var note = ProgressNote.Sign(
            clinicA.Id,
            patient.Id,
            DemoIds.ProfesionalA,
            "Evolucion demo: control de rutina. Texto ficticio.",
            time);
        var start = new DateTimeOffset(2026, 10, 23, 15, 0, 0, TimeSpan.Zero);
        var appointment = Appointment.Schedule(
            clinicA.Id,
            patient.Id,
            DemoIds.ProfesionalA,
            start,
            start.AddMinutes(30),
            [],
            time);
        var audit = ClinicalAccessAudit.Record(
            clinicA.Id,
            DemoIds.ProfesionalA,
            patient.Id,
            ClinicalAccessAction.Created,
            "ProgressNote",
            note.Id,
            time);

        db.AddRange(patient, plan, coverage, anamnesis, diagnosis, patientDiagnosis, note, appointment, audit);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedClinicBAsync(
        ConsultoriosDbContext db,
        TimeProvider time,
        Tenant clinicB,
        CancellationToken cancellationToken)
    {
        var patient = Patient.Create(clinicB.Id, "Demo", "Paciente Demo 2", "20000002");
        var plan = HealthPlan.Create(clinicB.Id, "OS Demo B");
        var coverage = PatientCoverage.Create(clinicB.Id, patient.Id, plan.Id, "AF-DEMO-0002");
        var anamnesis = Anamnesis.CreateEmpty(clinicB.Id, patient.Id, time);
        db.AddRange(patient, plan, coverage, anamnesis);
        await db.SaveChangesAsync(cancellationToken);
    }
}
