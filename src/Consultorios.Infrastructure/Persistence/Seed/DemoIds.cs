namespace Consultorios.Infrastructure.Persistence.Seed;

/// <summary>
/// Stable fictitious identifiers for local seed. Not real professionals or users.
/// </summary>
public static class DemoIds
{
    public static readonly Guid ProfesionalA = Guid.Parse("11111111-1111-7111-8111-111111111111");
    public static readonly Guid ProfesionalA2 = Guid.Parse("33333333-3333-7333-8333-333333333333");
    public static readonly Guid ProfesionalB = Guid.Parse("22222222-2222-7222-8222-222222222222");

    public const string AdminClinicA = "admin.clinicaa@demo.local";
    public const string ProfesionalClinicA = "profesional.clinicaa@demo.local";
    public const string Profesional2ClinicA = "profesional2.clinicaa@demo.local";
    public const string RecepcionClinicA = "recepcion.clinicaa@demo.local";
    public const string AdminClinicB = "admin.clinicab@demo.local";
    public const string ProfesionalClinicB = "profesional.clinicab@demo.local";
    public const string RecepcionClinicB = "recepcion.clinicab@demo.local";
}
