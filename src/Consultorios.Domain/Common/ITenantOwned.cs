namespace Consultorios.Domain.Common;

public interface ITenantOwned
{
    Guid TenantId { get; }
}
