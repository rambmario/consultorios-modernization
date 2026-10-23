namespace Consultorios.Domain.Patients;

public sealed class Address
{
    public string? Street { get; }
    public string? Number { get; }
    public string? Apartment { get; }
    public string? City { get; }
    public string? Province { get; }
    public string? PostalCode { get; }

    public Address(
        string? street,
        string? number,
        string? apartment,
        string? city,
        string? province,
        string? postalCode)
    {
        Street = Normalize(street);
        Number = Normalize(number);
        Apartment = Normalize(apartment);
        City = Normalize(city);
        Province = Normalize(province);
        PostalCode = Normalize(postalCode);
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
