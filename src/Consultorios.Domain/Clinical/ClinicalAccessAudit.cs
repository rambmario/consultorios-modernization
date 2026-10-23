using Consultorios.Domain.Common;

namespace Consultorios.Domain.Clinical;

public sealed class ClinicalAccessAudit : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid ActorUserId { get; private set; }
    public Guid PatientId { get; private set; }
    public ClinicalAccessAction Action { get; private set; }
    public string ResourceType { get; private set; } = string.Empty;
    public Guid ResourceId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }

    private ClinicalAccessAudit()
    {
    }

    public static ClinicalAccessAudit Record(
        Guid tenantId,
        Guid actorUserId,
        Guid patientId,
        ClinicalAccessAction action,
        string resourceType,
        Guid resourceId,
        TimeProvider time)
    {
        Guard.AgainstEmptyTenant(tenantId);
        ArgumentNullException.ThrowIfNull(time);
        Guard.AgainstEmpty(actorUserId, "ActorUserId");
        Guard.AgainstEmpty(patientId, "PatientId");
        Guard.AgainstEmpty(resourceId, "ResourceId");

        if (string.IsNullOrWhiteSpace(resourceType))
        {
            throw new DomainException("Resource type is required.");
        }

        return new ClinicalAccessAudit
        {
            TenantId = tenantId,
            ActorUserId = actorUserId,
            PatientId = patientId,
            Action = action,
            ResourceType = resourceType.Trim(),
            ResourceId = resourceId,
            OccurredAt = time.GetUtcNow()
        };
    }
}
