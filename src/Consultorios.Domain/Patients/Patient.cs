using Consultorios.Domain.Common;

namespace Consultorios.Domain.Patients;

public sealed class Patient : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public Address Address { get; private set; } = new(null, null, null, null, null, null);
    public DateTimeOffset? FirstConsultationAt { get; private set; }

    private Patient()
    {
    }

    public static Patient Create(
        Guid tenantId,
        string lastName,
        string firstName,
        string nationalId,
        string? phone = null,
        Address? address = null,
        DateTimeOffset? firstConsultationAt = null)
    {
        Guard.AgainstEmptyTenant(tenantId);

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(nationalId))
        {
            throw new DomainException("National ID is required.");
        }

        return new Patient
        {
            TenantId = tenantId,
            LastName = lastName.Trim(),
            FirstName = firstName.Trim(),
            NationalId = nationalId.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Address = address ?? new Address(null, null, null, null, null, null),
            FirstConsultationAt = firstConsultationAt
        };
    }

    public void UpdateDemographics(
        string lastName,
        string firstName,
        string nationalId,
        string? phone,
        Address? address)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(nationalId))
        {
            throw new DomainException("National ID is required.");
        }

        LastName = lastName.Trim();
        FirstName = firstName.Trim();
        NationalId = nationalId.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Address = address ?? Address;
    }

}
