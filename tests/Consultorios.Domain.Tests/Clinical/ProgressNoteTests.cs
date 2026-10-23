using System.Reflection;
using Consultorios.Domain.Clinical;
using Consultorios.Domain.Common;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace Consultorios.Domain.Tests.Clinical;

public class ProgressNoteTests
{
    private static readonly Guid ProfessionalA = Guid.CreateVersion7();

    [Fact]
    public void Sign_is_immediate_and_not_editable()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);

        var note = ProgressNote.Sign(
            tenant.Id,
            patient.Id,
            ProfessionalA,
            "Evolucion inicial",
            DomainFixtures.Time);

        note.Status.Should().Be(ProgressNoteStatus.Active);
        note.ProfessionalId.Should().Be(ProfessionalA);
        note.SignedAt.Should().Be(DomainFixtures.Time.GetUtcNow());
        note.Body.Should().Be("Evolucion inicial");

        typeof(ProgressNote)
            .GetProperty(nameof(ProgressNote.Body))!
            .SetMethod
            .Should()
            .NotBeNull()
            .And.Match<MethodInfo>(m => !m.IsPublic);
    }

    [Fact]
    public void Cancel_keeps_body_and_records_trail()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var note = ProgressNote.Sign(tenant.Id, patient.Id, ProfessionalA, "Texto firmado", DomainFixtures.Time);
        var later = new FakeTimeProvider(DomainFixtures.Time.GetUtcNow().AddHours(1));

        note.Cancel(ProfessionalA, "Carga errónea", later);

        note.Status.Should().Be(ProgressNoteStatus.Cancelled);
        note.Body.Should().Be("Texto firmado");
        note.CancelledByProfessionalId.Should().Be(ProfessionalA);
        note.CancellationReason.Should().Be("Carga errónea");
        note.CancelledAt.Should().Be(later.GetUtcNow());
    }

    [Fact]
    public void Correct_supersedes_original_and_signs_a_new_note()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var original = ProgressNote.Sign(tenant.Id, patient.Id, ProfessionalA, "Texto original", DomainFixtures.Time);
        var later = new FakeTimeProvider(DomainFixtures.Time.GetUtcNow().AddMinutes(30));

        var correction = original.Correct(ProfessionalA, "Texto corregido", later);

        original.Status.Should().Be(ProgressNoteStatus.Superseded);
        original.Body.Should().Be("Texto original");
        original.SupersededByProfessionalId.Should().Be(ProfessionalA);
        original.SupersededAt.Should().Be(later.GetUtcNow());

        correction.Status.Should().Be(ProgressNoteStatus.Active);
        correction.Body.Should().Be("Texto corregido");
        correction.CorrectsNoteId.Should().Be(original.Id);
        correction.SignedAt.Should().Be(later.GetUtcNow());
        correction.Id.Should().NotBe(original.Id);
    }

    [Fact]
    public void Cancelled_note_cannot_be_corrected_or_cancelled_again()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var note = ProgressNote.Sign(tenant.Id, patient.Id, ProfessionalA, "Texto", DomainFixtures.Time);
        note.Cancel(ProfessionalA, "Anulada", DomainFixtures.Time);

        var cancelAgain = () => note.Cancel(ProfessionalA, "Otra vez", DomainFixtures.Time);
        var correct = () => note.Correct(ProfessionalA, "Nuevo", DomainFixtures.Time);

        cancelAgain.Should().Throw<DomainException>();
        correct.Should().Throw<DomainException>();
    }

    [Fact]
    public void Another_professional_cannot_cancel_or_correct()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var other = Guid.CreateVersion7();
        var note = ProgressNote.Sign(tenant.Id, patient.Id, ProfessionalA, "Texto firmado", DomainFixtures.Time);

        var cancel = () => note.Cancel(other, "No es mia", DomainFixtures.Time);
        var correct = () => note.Correct(other, "Texto ajeno", DomainFixtures.Time);

        cancel.Should().Throw<DomainException>()
            .WithMessage("Only the signing professional can cancel or correct a progress note.");
        correct.Should().Throw<DomainException>()
            .WithMessage("Only the signing professional can cancel or correct a progress note.");
        note.Status.Should().Be(ProgressNoteStatus.Active);
        note.Body.Should().Be("Texto firmado");
    }

    [Fact]
    public void Sign_requires_body_and_professional()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);

        var noBody = () => ProgressNote.Sign(tenant.Id, patient.Id, ProfessionalA, " ", DomainFixtures.Time);
        var noProfessional = () => ProgressNote.Sign(tenant.Id, patient.Id, Guid.Empty, "Texto", DomainFixtures.Time);

        noBody.Should().Throw<DomainException>().WithMessage("Progress note body is required.");
        noProfessional.Should().Throw<DomainException>().WithMessage("ProfessionalId is required.");
    }
}
