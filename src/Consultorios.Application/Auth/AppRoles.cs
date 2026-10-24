namespace Consultorios.Application.Auth;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Profesional = "Profesional";
    public const string Recepcion = "Recepcion";

    public static readonly string[] All = [Admin, Profesional, Recepcion];
}
