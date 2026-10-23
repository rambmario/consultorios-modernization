using Consultorios.Domain.Clinical;
using Consultorios.Domain.Common;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace Consultorios.Domain.Tests.Clinical;

public class PatientDiagnosisTests
{
    private static readonly Guid ProfessionalA = Guid.CreateVersion7();

    [Fact]
    public void Open_creates_current_validity()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var diagnosis = Diagnosis.Create(tenant.Id, "HTA esencial");

        var entry = PatientDiagnosis.Open(
            tenant.Id,
            patient.Id,
            diagnosis.Id,
            ProfessionalA,
            [],
            DomainFixtures.Time);

        entry.IsCurrent.Should().BeTrue();
        entry.ValidFrom.Should().Be(DomainFixtures.Time.GetUtcNow());
        entry.ValidTo.Should().BeNull();
        entry.ProfessionalId.Should().Be(ProfessionalA);
        entry.RecordedAt.Should().Be(DomainFixtures.Time.GetUtcNow());
    }

    [Fact]
    public void Cannot_open_the_same_diagnosis_twice_while_current()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var diagnosis = Diagnosis.Create(tenant.Id, "Diabetes");
        var open = PatientDiagnosis.Open(tenant.Id, patient.Id, diagnosis.Id, ProfessionalA, [], DomainFixtures.Time);

        var act = () => PatientDiagnosis.Open(
            tenant.Id,
            patient.Id,
            diagnosis.Id,
            ProfessionalA,
            [open],
            DomainFixtures.Time);

        act.Should().Throw<DomainException>().WithMessage("This diagnosis is already current for the patient.");
    }

    [Fact]
    public void Close_ends_validity_and_allows_a_later_history_row()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var diagnosis = Diagnosis.Create(tenant.Id, "FA paroxística");
        var first = PatientDiagnosis.Open(tenant.Id, patient.Id, diagnosis.Id, ProfessionalA, [], DomainFixtures.Time);
        var later = new FakeTimeProvider(DomainFixtures.Time.GetUtcNow().AddDays(10));

        first.Close(later);

        first.IsCurrent.Should().BeFalse();
        first.ValidTo.Should().Be(later.GetUtcNow());

        var second = PatientDiagnosis.Open(
            tenant.Id,
            patient.Id,
            diagnosis.Id,
            ProfessionalA,
            [first],
            later);

        second.IsCurrent.Should().BeTrue();
        second.Id.Should().NotBe(first.Id);
    }

    [Fact]
    public void Different_diagnoses_can_be_current_at_once()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var hta = Diagnosis.Create(tenant.Id, "HTA");
        var db = Diagnosis.Create(tenant.Id, "DBT");
        var first = PatientDiagnosis.Open(tenant.Id, patient.Id, hta.Id, ProfessionalA, [], DomainFixtures.Time);

        var second = PatientDiagnosis.Open(tenant.Id, patient.Id, db.Id, ProfessionalA, [first], DomainFixtures.Time);

        first.IsCurrent.Should().BeTrue();
        second.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void Close_twice_fails()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var diagnosis = Diagnosis.Create(tenant.Id, "Asma");
        var entry = PatientDiagnosis.Open(tenant.Id, patient.Id, diagnosis.Id, ProfessionalA, [], DomainFixtures.Time);
        entry.Close(DomainFixtures.Time);

        var act = () => entry.Close(DomainFixtures.Time);
        act.Should().Throw<DomainException>().WithMessage("Diagnosis validity is already closed.");
    }

    [Fact]
    public void Validity_cannot_start_in_the_future()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var diagnosis = Diagnosis.Create(tenant.Id, "IAM");
        var future = DomainFixtures.Time.GetUtcNow().AddDays(1);

        var act = () => PatientDiagnosis.Open(
            tenant.Id,
            patient.Id,
            diagnosis.Id,
            ProfessionalA,
            [],
            DomainFixtures.Time,
            future);

        act.Should().Throw<DomainException>().WithMessage("Diagnosis validity cannot start in the future.");
    }
}
