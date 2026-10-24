namespace Consultorios.Api.IntegrationTests.Infrastructure;

internal static class TestAuth
{
    public const string JwtSigningKey = "TEST_ONLY_signing_key_32chars_min!!";
    public const string AdminPassword = "AdminPass123";
    public const string ProfesionalPassword = "ProfesionalPass123";
    public const string ReceptionPassword = "RecepcionPass123";

    public static void ApplyEnvironment()
    {
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        Environment.SetEnvironmentVariable("DEMO_PASSWORD_ADMIN", AdminPassword);
        Environment.SetEnvironmentVariable("DEMO_PASSWORD_PROFESIONAL", ProfesionalPassword);
        Environment.SetEnvironmentVariable("DEMO_PASSWORD_RECEPTION", ReceptionPassword);
    }
}
