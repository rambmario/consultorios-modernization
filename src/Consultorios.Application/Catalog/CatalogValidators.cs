using FluentValidation;

namespace Consultorios.Application.Catalog;

public sealed class CreateHealthPlanRequestValidator : AbstractValidator<CreateHealthPlanRequest>
{
    public CreateHealthPlanRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateDiagnosisRequestValidator : AbstractValidator<CreateDiagnosisRequest>
{
    public CreateDiagnosisRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Details).MaximumLength(2000);
    }
}
