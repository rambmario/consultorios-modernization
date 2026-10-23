# ADR 0004 — Persistencia con EF Core y PostgreSQL

**Estado:** Aceptado  
**Fecha:** 2026-10-22  
**Decisores:** Mario

---

## Contexto

Paso 3 de la Fase 1: el dominio ya modela ficha, HC, evoluciones y agenda. Hace falta persistencia relacional con aislamiento por tenant, integridad referencial estricta y una garantía de base de datos para que dos turnos del mismo profesional no se pisen bajo carrera.

## Decisión

- **EF Core 10 + PostgreSQL 16 + Npgsql** en `Consultorios.Infrastructure` (`ConsultoriosDbContext`, Fluent API, migraciones).
- **Filtros globales fail-closed** por `TenantId` vía `ITenantContext` / `AmbientTenantContext` (ADR 0003).
- **FKs con `DeleteBehavior.Restrict`**, incluido `Tenant`. Sin cascade de borrado clínico.
- **`IConsultoriosDbContext` en Application** expone `DbSet<>` de EF. Trade-off aceptado: los casos de uso (Paso 4+) evitan un repositorio extra; Application toma una dependencia de abstracciones de EF, no de Infrastructure.
- **docker-compose** con PostgreSQL 16; seed 100 % ficticio (`ClinicaA`, `ClinicaB`, `Paciente Demo 1`); health liveness (`/health`) vs readiness (`/health/ready`).
- Secretos solo en `.env` / `.env.example`. Design-time factory para `dotnet ef`.

### Exclusion constraint de turnos

El dominio rechaza solapes de `Scheduled`/`Confirmed` (`OccupiesSlot`). Eso no alcanza si dos `SaveChanges` corren en paralelo con la colección `existing` vacía.

PostgreSQL cierra esa carrera:

```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;

ALTER TABLE "Appointments"
  ADD CONSTRAINT "EX_Appointments_ProfessionalOverlap"
  EXCLUDE USING gist (
    "TenantId" WITH =,
    "ProfessionalId" WITH =,
    tstzrange("StartsAt", "EndsAt", '[)') WITH &&
  )
  WHERE ("Status" IN (0, 1));
```

- `btree_gist` permite igualdad (`=`) de `uuid` junto al overlap de rangos en GiST.
- `tstzrange(..., '[)')` coincide con el intervalo semiabierto del dominio (adyacentes no solapan).
- `WHERE Status IN (0, 1)` = `Scheduled` / `Confirmed`. Un turno cancelado, atendido o no-show libera el hueco.
- `OccupiesSlot` no se mapea: la cláusula SQL usa `Status`.

Un integration test con **Testcontainers.PostgreSql** dispara dos `Schedule` + `SaveChanges` en paralelo. Debe quedar **una** fila y el perdedor con `DbUpdateException` / SQLSTATE `23P01` (`exclusion_violation`). Docker es requisito: **0 skipped**.

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Solo regla de dominio | Pierde la carrera; dos contextos no ven el turno del otro. |
| `SERIALIZABLE` / lock pesimista en Application | Más contention y fácil de olvidar en un caso de uso nuevo. |
| Unique index en start time | No cubre solape parcial (14:00–15:00 vs 14:30–15:30). |
| Esquema por tenant | Ya descartado (shared DB + `TenantId`). |
| Dapper / SQL crudo | Más boilerplate; no alinea al ritmo de Fase 1. |
| Test skippeable si no hay Docker | El dueño pidió 0 skipped; el test falla si Docker no está. |

## Consecuencias

- Migraciones viven en Infrastructure; la exclusion se agrega en SQL de `InitialCreate` (EF no modela `EXCLUDE` en Fluent API).
- CI y local necesitan Docker para la suite de integración.
- Identity/JWT (Paso 4) reutilizará este `DbContext`; `ProfessionalId` hoy es un `Guid` opaco, sin tabla de profesional.
