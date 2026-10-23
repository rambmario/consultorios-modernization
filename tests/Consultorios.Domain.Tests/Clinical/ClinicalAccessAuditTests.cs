using System.Reflection;
using Consultorios.Domain.Clinical;
using Consultorios.Domain.Common;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;

namespace Consultorios.Domain.Tests.Clinical;

public class ClinicalAccessAuditTests
{
    [Fact]
    public void Record_is_append_only_snapshot()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var actor = Guid.CreateVersion7();
        var resource = Guid.CreateVersion7();

        var entry = ClinicalAccessAudit.Record(
            tenant.Id,
            actor,
            patient.Id,
            ClinicalAccessAction.Viewed,
            nameof(ProgressNote),
            resource,
            DomainFixtures.Time);

        entry.Action.Should().Be(ClinicalAccessAction.Viewed);
        entry.ActorUserId.Should().Be(actor);
        entry.PatientId.Should().Be(patient.Id);
        entry.ResourceType.Should().Be(nameof(ProgressNote));
        entry.ResourceId.Should().Be(resource);
        entry.OccurredAt.Should().Be(DomainFixtures.Time.GetUtcNow());

        typeof(ClinicalAccessAudit)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Should()
            .BeEmpty("audit entries must not expose mutators");
    }

    [Fact]
    public void Record_requires_actor_and_resource()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);

        var act = () => ClinicalAccessAudit.Record(
            tenant.Id,
            Guid.Empty,
            patient.Id,
            ClinicalAccessAction.Updated,
            "Anamnesis",
            Guid.CreateVersion7(),
            DomainFixtures.Time);

        act.Should().Throw<DomainException>().WithMessage("ActorUserId is required.");
    }
}
