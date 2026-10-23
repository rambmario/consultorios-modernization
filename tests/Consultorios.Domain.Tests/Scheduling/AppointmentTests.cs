using Consultorios.Domain.Common;
using Consultorios.Domain.Scheduling;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;

namespace Consultorios.Domain.Tests.Scheduling;

public class AppointmentTests
{
    private static readonly Guid ProfessionalA = Guid.CreateVersion7();
    private static readonly Guid ProfessionalB = Guid.CreateVersion7();

    private static Appointment Slot(
        Guid tenantId,
        Guid patientId,
        Guid professionalId,
        int startHour,
        int endHour,
        IReadOnlyCollection<Appointment>? existing = null)
        => Appointment.Schedule(
            tenantId,
            patientId,
            professionalId,
            DomainFixtures.Time.GetUtcNow().Date.AddHours(startHour),
            DomainFixtures.Time.GetUtcNow().Date.AddHours(endHour),
            existing ?? [],
            DomainFixtures.Time);

    [Fact]
    public void Schedule_starts_as_Scheduled()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);

        var appointment = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11);

        appointment.Status.Should().Be(AppointmentStatus.Scheduled);
        appointment.ProfessionalId.Should().Be(ProfessionalA);
        appointment.OccupiesSlot.Should().BeTrue();
    }

    [Fact]
    public void Same_professional_cannot_overlap()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var first = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11);

        var act = () => Slot(tenant.Id, patient.Id, ProfessionalA, 10, 12, [first]);

        act.Should().Throw<DomainException>()
            .WithMessage("The professional already has an appointment in that time range.");
    }

    [Fact]
    public void Adjacent_slots_do_not_overlap()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var first = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11);

        var second = Slot(tenant.Id, patient.Id, ProfessionalA, 11, 12, [first]);

        second.StartsAt.Should().Be(first.EndsAt);
    }

    [Fact]
    public void Another_professional_may_use_the_same_slot()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var first = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11);

        var second = Slot(tenant.Id, patient.Id, ProfessionalB, 10, 11, [first]);

        second.ProfessionalId.Should().Be(ProfessionalB);
    }

    [Fact]
    public void Cancelled_slot_can_be_reused()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var first = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11);
        first.Cancel();

        var second = Slot(tenant.Id, patient.Id, ProfessionalA, 10, 11, [first]);

        second.Status.Should().Be(AppointmentStatus.Scheduled);
        first.OccupiesSlot.Should().BeFalse();
    }

    [Fact]
    public void Lifecycle_Scheduled_Confirmed_Attended()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var appointment = Slot(tenant.Id, patient.Id, ProfessionalA, 9, 10);

        appointment.Confirm();
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);

        appointment.MarkAttended();
        appointment.Status.Should().Be(AppointmentStatus.Attended);
        appointment.OccupiesSlot.Should().BeFalse();
    }

    [Fact]
    public void Terminal_statuses_cannot_change()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var attended = Slot(tenant.Id, patient.Id, ProfessionalA, 9, 10);
        attended.MarkAttended();

        var act = () => attended.Cancel();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void End_must_be_after_start()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var start = DomainFixtures.Time.GetUtcNow();

        var act = () => Appointment.Schedule(
            tenant.Id,
            patient.Id,
            ProfessionalA,
            start,
            start,
            [],
            DomainFixtures.Time);

        act.Should().Throw<DomainException>().WithMessage("Appointment end must be after start.");
    }
}
