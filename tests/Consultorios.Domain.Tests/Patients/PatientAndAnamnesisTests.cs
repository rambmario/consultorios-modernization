using Consultorios.Domain.Common;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;

namespace Consultorios.Domain.Tests.Patients;

public class PatientAndAnamnesisTests
{
    [Fact]
    public void Patient_uses_simple_address()
    {
        var tenant = DomainFixtures.ClinicA();
        var address = new Address("Calle Falsa", "123", "B", "Ciudad Demo", "Buenos Aires", "1000");

        var patient = Patient.Create(
            tenant.Id,
            "Demo",
            "Paciente Demo 1",
            "20000001",
            "1100000000",
            address,
            DomainFixtures.Time.GetUtcNow());

        patient.TenantId.Should().Be(tenant.Id);
        patient.Address.Street.Should().Be("Calle Falsa");
        patient.Address.Number.Should().Be("123");
        patient.FirstConsultationAt.Should().Be(DomainFixtures.Time.GetUtcNow());
    }

    [Fact]
    public void Patient_requires_identity_fields()
    {
        var tenantId = Guid.CreateVersion7();
        var act = () => Patient.Create(tenantId, " ", "Nombre", "20000001");
        act.Should().Throw<DomainException>().WithMessage("Last name is required.");
    }

    [Fact]
    public void Anamnesis_exposes_six_long_section_names()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var anamnesis = Anamnesis.CreateEmpty(tenant.Id, patient.Id, DomainFixtures.Time);

        anamnesis.ReplaceSections(
            "Dolor precordial",
            "Inicio hace 2 dias",
            "HTA",
            "Padre IAM",
            "Ritmo regular",
            "Ex tabaquista",
            DomainFixtures.Time);

        var sections = anamnesis.SectionsByLongName();
        sections.Keys.Should().Equal(
            AnamnesisSections.ChiefComplaint,
            AnamnesisSections.CurrentIllnessHistory,
            AnamnesisSections.PersonalPathologicalHistory,
            AnamnesisSections.FamilyHistory,
            AnamnesisSections.PhysicalExamination,
            AnamnesisSections.HabitHistory);

        AnamnesisSections.ChiefComplaint.Should().Be("Motivo de consulta");
        AnamnesisSections.CurrentIllnessHistory.Should().Be("Antecedentes de la enfermedad actual");
        AnamnesisSections.PersonalPathologicalHistory.Should().Be("Antecedentes personales patológicos");
        AnamnesisSections.FamilyHistory.Should().Be("Antecedentes familiares");
        AnamnesisSections.PhysicalExamination.Should().Be("Examen físico");
        AnamnesisSections.HabitHistory.Should().Be("Antecedentes de hábitos");
        anamnesis.ChiefComplaint.Should().Be("Dolor precordial");
    }
}
