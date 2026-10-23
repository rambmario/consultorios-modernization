namespace Consultorios.Domain.Patients;

public sealed class Address
{
    public string? Street { get; private set; }
    public string? Number { get; private set; }
    public string? Apartment { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }
    public string? PostalCode { get; private set; }

    private Address()
    {
    }

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
