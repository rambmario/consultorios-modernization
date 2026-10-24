using FluentValidation;

namespace Consultorios.Application.Scheduling;

public sealed class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProfessionalId).NotEmpty();
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
    }
}
