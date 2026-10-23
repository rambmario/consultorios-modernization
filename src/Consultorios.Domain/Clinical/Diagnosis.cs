using Consultorios.Domain.Common;
using Consultorios.Domain.Patients;

namespace Consultorios.Domain.Clinical;

public sealed class Diagnosis : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Details { get; private set; }

    private Diagnosis()
    {
    }

    public static Diagnosis Create(Guid tenantId, string name, string? details = null)
    {
        Patient.EnsureTenant(tenantId);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Diagnosis name is required.");
        }

        return new Diagnosis
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim()
        };
    }
}
