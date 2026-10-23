using Consultorios.Api.IntegrationTests.Infrastructure;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tenancy;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Consultorios.Api.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class AppointmentExclusionTests
{
    private readonly PostgresFixture _postgres;

    public AppointmentExclusionTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task Parallel_overlapping_saves_keep_only_one_appointment()
    {
        await using var provider = PersistenceTestServices.Create(_postgres.ConnectionString);
        var (tenantId, patientId, professionalId) = await SeedProfessionalDayAsync(provider);

        var start = new DateTimeOffset(2026, 10, 25, 15, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);

        var tasks = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(async () =>
            {
                await using var scope = provider.CreateAsyncScope();
                var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
                var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
                var time = scope.ServiceProvider.GetRequiredService<TimeProvider>();
                tenantContext.SetTenant(tenantId);

                var appointment = Appointment.Schedule(
                    tenantId,
                    patientId,
                    professionalId,
                    start,
                    end,
                    [],
                    time);
                db.Appointments.Add(appointment);

                try
                {
                    await db.SaveChangesAsync();
                    return new SaveAttempt(Succeeded: true, SqlState: null);
                }
                catch (DbUpdateException ex)
                {
                    return new SaveAttempt(Succeeded: false, SqlState: FindSqlState(ex));
                }
            }))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        results.Count(r => r.Succeeded).Should().Be(1);
        results.Should().ContainSingle(r => !r.Succeeded);
        results.Single(r => !r.Succeeded).SqlState.Should().Be(PostgresErrorCodes.ExclusionViolation);

        await using var verify = provider.CreateAsyncScope();
        var verifyTenant = verify.ServiceProvider.GetRequiredService<AmbientTenantContext>();
        var verifyDb = verify.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        verifyTenant.SetTenant(tenantId);

        var saved = await verifyDb.Appointments
            .Where(a => a.ProfessionalId == professionalId)
            .ToListAsync();

        saved.Should().ContainSingle();
        saved[0].StartsAt.Should().Be(start);
        saved[0].EndsAt.Should().Be(end);
    }

    [Fact]
    public async Task Cancelled_slot_can_be_reused_by_another_appointment()
    {
        await using var provider = PersistenceTestServices.Create(_postgres.ConnectionString);
        var (tenantId, patientId, professionalId) = await SeedProfessionalDayAsync(provider);
        var start = new DateTimeOffset(2026, 10, 26, 15, 0, 0, TimeSpan.Zero);

        await using (var scope = provider.CreateAsyncScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
            var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
            var time = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            tenantContext.SetTenant(tenantId);

            var first = Appointment.Schedule(tenantId, patientId, professionalId, start, start.AddHours(1), [], time);
            db.Appointments.Add(first);
            await db.SaveChangesAsync();

            first.Cancel();
            await db.SaveChangesAsync();

            var replacement = Appointment.Schedule(
                tenantId,
                patientId,
                professionalId,
                start,
                start.AddHours(1),
                [],
                time);
            db.Appointments.Add(replacement);
            await db.SaveChangesAsync();
        }

        await using var verify = provider.CreateAsyncScope();
        var verifyTenant = verify.ServiceProvider.GetRequiredService<AmbientTenantContext>();
        var verifyDb = verify.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        verifyTenant.SetTenant(tenantId);

        var appointments = await verifyDb.Appointments
            .Where(a => a.ProfessionalId == professionalId)
            .ToListAsync();

        appointments.Should().HaveCount(2);
        appointments.Should().ContainSingle(a => a.Status == AppointmentStatus.Cancelled);
        appointments.Should().ContainSingle(a => a.Status == AppointmentStatus.Scheduled);
    }

    private async Task<(Guid TenantId, Guid PatientId, Guid ProfessionalId)> SeedProfessionalDayAsync(
        ServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
        tenantContext.Clear();

        var tenant = Tenant.Create($"ClinicaA Agenda {Guid.CreateVersion7():N}");
        var patient = Patient.Create(tenant.Id, "Demo", "Paciente Demo 1", $"22{Guid.CreateVersion7():N}"[..8]);
        db.Tenants.Add(tenant);
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        return (tenant.Id, patient.Id, Guid.CreateVersion7());
    }

    private static string? FindSqlState(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres)
            {
                return postgres.SqlState;
            }
        }

        return null;
    }

    private sealed record SaveAttempt(bool Succeeded, string? SqlState);
}
