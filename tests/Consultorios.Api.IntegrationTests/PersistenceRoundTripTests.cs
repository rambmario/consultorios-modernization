using Consultorios.Api.IntegrationTests.Infrastructure;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Consultorios.Infrastructure.Persistence;
using Consultorios.Infrastructure.Tenancy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Consultorios.Api.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class PersistenceRoundTripTests
{
    private readonly PostgresFixture _postgres;

    public PersistenceRoundTripTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task Patient_address_round_trips_as_owned_type()
    {
        await using var provider = PersistenceTestServices.Create(_postgres.ConnectionString);
        Patient patient;
        Tenant tenant;

        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
            var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
            tenantContext.Clear();

            tenant = Tenant.Create($"ClinicaA Roundtrip {Guid.CreateVersion7():N}");
            patient = Patient.Create(
                tenant.Id,
                "Demo",
                "Paciente Demo 1",
                $"23{Guid.CreateVersion7():N}"[..8],
                address: new Address("Calle Falsa", "123", "B", "Ciudad Demo", "Provincia Demo", "1000"));
            db.Tenants.Add(tenant);
            db.Patients.Add(patient);
            await db.SaveChangesAsync();
        }

        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
            var tenantContext = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
            tenantContext.SetTenant(tenant.Id);

            var loaded = await db.Patients.SingleAsync(p => p.Id == patient.Id);
            loaded.Address.Street.Should().Be("Calle Falsa");
            loaded.Address.Number.Should().Be("123");
            loaded.Address.Apartment.Should().Be("B");
            loaded.Address.City.Should().Be("Ciudad Demo");
        }
    }
}
