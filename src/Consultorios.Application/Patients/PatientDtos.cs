namespace Consultorios.Application.Patients;

public sealed record AddressDto(
    string? Street,
    string? Number,
    string? Apartment,
    string? City,
    string? Province,
    string? PostalCode);

public sealed record CoverageDto(
    Guid HealthPlanId,
    string HealthPlanName,
    string AffiliationNumber);

public sealed record PatientResponse(
    Guid Id,
    string LastName,
    string FirstName,
    string NationalId,
    string? Phone,
    AddressDto Address,
    DateTimeOffset? FirstConsultationAt,
    CoverageDto? Coverage);

public sealed record CreatePatientRequest(
    string LastName,
    string FirstName,
    string NationalId,
    string? Phone,
    AddressDto? Address,
    DateTimeOffset? FirstConsultationAt,
    Guid? HealthPlanId,
    string? AffiliationNumber);

public sealed record UpdatePatientRequest(
    string LastName,
    string FirstName,
    string NationalId,
    string? Phone,
    AddressDto? Address,
    Guid? HealthPlanId,
    string? AffiliationNumber);
