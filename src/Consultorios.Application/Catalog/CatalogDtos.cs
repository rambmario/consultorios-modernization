namespace Consultorios.Application.Catalog;

public sealed record HealthPlanResponse(Guid Id, string Name);

public sealed record CreateHealthPlanRequest(string Name);

public sealed record DiagnosisCatalogResponse(Guid Id, string Name, string? Details);

public sealed record CreateDiagnosisRequest(string Name, string? Details);
