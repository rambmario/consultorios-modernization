using Consultorios.Domain.Common;
using Consultorios.Domain.Patients;

namespace Consultorios.Domain.HealthPlans;

public sealed class PatientCoverage : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid HealthPlanId { get; private set; }
    public string AffiliationNumber { get; private set; } = string.Empty;

    private PatientCoverage()
    {
    }

    public static PatientCoverage Create(
        Guid tenantId,
        Guid patientId,
        Guid healthPlanId,
        string affiliationNumber)
    {
        Patient.EnsureTenant(tenantId);

        if (patientId == Guid.Empty)
        {
            throw new DomainException("PatientId is required.");
        }

        if (healthPlanId == Guid.Empty)
        {
            throw new DomainException("HealthPlanId is required.");
        }

        if (string.IsNullOrWhiteSpace(affiliationNumber))
        {
            throw new DomainException("Affiliation number is required.");
        }

        return new PatientCoverage
        {
            TenantId = tenantId,
            PatientId = patientId,
            HealthPlanId = healthPlanId,
            AffiliationNumber = affiliationNumber.Trim()
        };
    }

    public void Change(Guid healthPlanId, string affiliationNumber)
    {
        if (healthPlanId == Guid.Empty)
        {
            throw new DomainException("HealthPlanId is required.");
        }

        if (string.IsNullOrWhiteSpace(affiliationNumber))
        {
            throw new DomainException("Affiliation number is required.");
        }

        HealthPlanId = healthPlanId;
        AffiliationNumber = affiliationNumber.Trim();
    }
}
