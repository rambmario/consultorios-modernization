using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.Extensions.Time.Testing;

namespace Consultorios.Domain.Tests.Support;

internal static class DomainFixtures
{
    public static readonly FakeTimeProvider Time = new(new DateTimeOffset(2026, 10, 22, 15, 0, 0, TimeSpan.Zero));

    public static Tenant ClinicA() => Tenant.Create("ClinicaA");

    public static Patient DemoPatient(Guid tenantId)
        => Patient.Create(tenantId, "Demo", "Paciente Demo 1", "20000001");
}
