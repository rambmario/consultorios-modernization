using Microsoft.AspNetCore.Identity;

namespace Consultorios.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
}
