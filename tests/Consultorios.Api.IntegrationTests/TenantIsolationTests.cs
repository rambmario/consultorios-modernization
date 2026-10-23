using Consultorios.Api.IntegrationTests.Infrastructure;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tenancy;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Consultorios.Api.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class TenantIsolationTests
{
    private readonly PostgresFixture _postgres;

    public TenantIsolationTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task Tenant_A_queries_exclude_tenant_B_and_empty_without_tenant()
    {
        await using var provider = PersistenceTestServices.Create(_postgres.ConnectionString);
        await using var seedScope = provider.CreateAsyncScope();
        var seedDb = seedScope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        var time = seedScope.ServiceProvider.GetRequiredService<TimeProvider>();
        var seedTenant = seedScope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
        seedTenant.Clear();

        var tenantA = Tenant.Create($"ClinicaA Isolation {Guid.CreateVersion7():N}");
        var tenantB = Tenant.Create($"ClinicaB Isolation {Guid.CreateVersion7():N}");
        seedDb.Tenants.AddRange(tenantA, tenantB);

        var patientA = Patient.Create(tenantA.Id, "Demo", "Paciente Demo 1", "21000001");
        var patientB = Patient.Create(tenantB.Id, "Demo", "Paciente Demo 2", "21000002");
        seedDb.Patients.AddRange(patientA, patientB);

        var start = new DateTimeOffset(2026, 10, 24, 15, 0, 0, TimeSpan.Zero);
        var appointmentA = Appointment.Schedule(
            tenantA.Id,
            patientA.Id,
            Guid.CreateVersion7(),
            start,
            start.AddMinutes(30),
            [],
            time);
        var appointmentB = Appointment.Schedule(
            tenantB.Id,
            patientB.Id,
            Guid.CreateVersion7(),
            start,
            start.AddMinutes(30),
            [],
            time);
        seedDb.Appointments.AddRange(appointmentA, appointmentB);
        await seedDb.SaveChangesAsync();

        await using (var scopeA = provider.CreateAsyncScope())
        {
            var tenantContext = scopeA.ServiceProvider.GetRequiredService<AmbientTenantContext>();
            var db = scopeA.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
            tenantContext.SetTenant(tenantA.Id);

            var patients = await db.Patients.ToListAsync();
            var appointments = await db.Appointments.ToListAsync();

            patients.Should().ContainSingle(p => p.Id == patientA.Id);
            patients.Should().NotContain(p => p.Id == patientB.Id);
            appointments.Should().ContainSingle(a => a.Id == appointmentA.Id);
            appointments.Should().NotContain(a => a.Id == appointmentB.Id);
        }

        await using (var scopeNone = provider.CreateAsyncScope())
        {
            var tenantContext = scopeNone.ServiceProvider.GetRequiredService<AmbientTenantContext>();
            var db = scopeNone.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
            tenantContext.Clear();

            (await db.Patients.CountAsync()).Should().Be(0);
            (await db.Appointments.CountAsync()).Should().Be(0);
        }
    }
}
