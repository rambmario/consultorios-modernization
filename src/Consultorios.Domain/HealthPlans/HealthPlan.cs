using Consultorios.Domain.Common;

namespace Consultorios.Domain.HealthPlans;

public sealed class HealthPlan : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private HealthPlan()
    {
    }

    public static HealthPlan Create(Guid tenantId, string name)
    {
        Guard.AgainstEmptyTenant(tenantId);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Health plan name is required.");
        }

        return new HealthPlan
        {
            TenantId = tenantId,
            Name = name.Trim()
        };
    }
}
