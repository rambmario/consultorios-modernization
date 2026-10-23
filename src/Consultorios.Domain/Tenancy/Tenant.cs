using Consultorios.Domain.Common;

namespace Consultorios.Domain.Tenancy;

public sealed class Tenant : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Tenant()
    {
    }

    public static Tenant Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Tenant name is required.");
        }

        return new Tenant { Name = name.Trim() };
    }
}
