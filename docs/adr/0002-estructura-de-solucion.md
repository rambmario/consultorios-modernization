# ADR 0002 — Estructura de la solución (.NET 10 Clean Architecture)

**Estado:** Aceptado  
**Fecha:** 2026-10-22  
**Decisores:** Mario

---

## Contexto

Fase 1 necesita una base de código moderna antes del dominio clínico. Hay que fijar capas, dependencias y layout del repo para que los pasos siguientes (dominio, EF/PostgreSQL, API, React) encajen sin rearmar la solución.

## Decisión

Usar **Clean Architecture** en .NET 10 con esta estructura:

```
Consultorios.sln
src/
  Consultorios.Domain          # entidades y reglas; sin dependencias de infra
  Consultorios.Application     # casos de uso; solo Domain
  Consultorios.Infrastructure  # EF, Identity, externos; Application + Domain
  Consultorios.Api             # host HTTP; Application + Infrastructure
tests/
  Consultorios.Domain.Tests
  Consultorios.Application.Tests
  Consultorios.Api.IntegrationTests
Directory.Build.props          # nullable, TreatWarningsAsErrors, LangVersion latest
Directory.Packages.props       # Central Package Management
.editorconfig
global.json                    # SDK 10.0.x
nuget.config                   # solo nuget.org
```

**Dirección de dependencias:** Domain ← Application ← Infrastructure; Api → Application + Infrastructure. Nunca Domain/Application → Infrastructure.

**Tests:** xUnit + FluentAssertions. Domain y Application unitarios; Api con `WebApplicationFactory`. Integration tests con Testcontainers (PostgreSQL) desde el Paso 3.

**Desde el día uno (lecciones de Pymen):** `Guid.CreateVersion7()` en `Entity`, `DomainException`, `ITenantOwned`, `TimeProvider` registrado en Infrastructure.

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Monolito único proyecto | Acopla UI/persistencia al dominio desde el día 1 |
| Vertical slice sin capas nombradas | Válido más adelante; ahora conviene capas explícitas para portfolio/enseñanza |
| Formato `.slnx` por defecto del SDK 10 | Menos soporte en tooling; se usa `.sln` clásico |

## Consecuencias

**Positivas:** reglas de negocio aislables; tests de dominio sin DB; CI simple (`dotnet build` / `dotnet test`); una sola versión de cada paquete (CPM).

**Negativas:** más proyectos que un starter mínimo; Application/Infrastructure empiezan casi vacíos hasta Pasos 2–4.

**Seguimiento:** dominio clínico (Paso 2), EF + tenancy fail-closed (Paso 3), Identity/JWT (Paso 4). El trade-off de exponer EF en Application se documenta cuando se introduzca `IConsultoriosDbContext`.
