namespace Consultorios.Domain.Common;

public static class Guard
{
    public static void AgainstEmpty(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }
    }

    public static void AgainstEmptyTenant(Guid tenantId)
        => AgainstEmpty(tenantId, "TenantId");
}
