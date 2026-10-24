using FluentValidation;

namespace Consultorios.Application.Patients;

public sealed class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.NationalId).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Phone).MaximumLength(40);
        RuleFor(x => x.AffiliationNumber)
            .NotEmpty()
            .When(x => x.HealthPlanId is not null);
    }
}

public sealed class UpdatePatientRequestValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientRequestValidator()
    {
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.NationalId).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Phone).MaximumLength(40);
        RuleFor(x => x.AffiliationNumber)
            .NotEmpty()
            .When(x => x.HealthPlanId is not null);
    }
}
