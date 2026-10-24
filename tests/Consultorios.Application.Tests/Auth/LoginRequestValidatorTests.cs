using Consultorios.Application.Auth;
using FluentAssertions;

namespace Consultorios.Application.Tests.Auth;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Rejects_empty_email_and_short_password()
    {
        var result = _validator.Validate(new LoginRequest("", "123"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Accepts_demo_credentials_shape()
    {
        var result = _validator.Validate(new LoginRequest("profesional.clinicaa@demo.local", "ProfesionalPass123"));
        result.IsValid.Should().BeTrue();
    }
}
