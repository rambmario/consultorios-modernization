using System.ComponentModel.DataAnnotations;

namespace Consultorios.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "consultorios";
    public string Audience { get; set; } = "consultorios-api";

    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
