using FluentValidation;

namespace Consultorios.Application.Clinical;

public sealed class SignProgressNoteRequestValidator : AbstractValidator<SignProgressNoteRequest>
{
    public SignProgressNoteRequestValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
    }
}

public sealed class CancelProgressNoteRequestValidator : AbstractValidator<CancelProgressNoteRequest>
{
    public CancelProgressNoteRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public sealed class CorrectProgressNoteRequestValidator : AbstractValidator<CorrectProgressNoteRequest>
{
    public CorrectProgressNoteRequestValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
    }
}

public sealed class OpenPatientDiagnosisRequestValidator : AbstractValidator<OpenPatientDiagnosisRequest>
{
    public OpenPatientDiagnosisRequestValidator()
    {
        RuleFor(x => x.DiagnosisId).NotEmpty();
    }
}
