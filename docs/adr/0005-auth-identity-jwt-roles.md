# ADR 0005 — Autenticación Identity + JWT, roles y auditoría de HC

**Estado:** Aceptado  
**Fecha:** 2026-10-24  
**Decisores:** Mario

---

## Contexto

Paso 4 de la Fase 1: la API clínica necesita login multi-tenant, roles Admin / Profesional / Recepción y traza de quién vio o cambió la historia clínica. El cliente no puede elegir el tenant.

## Decisión

- **ASP.NET Identity** en el mismo `ConsultoriosDbContext` (`ApplicationUser : IdentityUser<Guid>` con `TenantId`). El `Id` del profesional es el `ProfessionalId` de evoluciones y turnos.
- **JWT** Bearer; claim obligatorio `tenant_id`. El middleware carga `ITenantContext` e `ICurrentUser`; sin claim → **401**.
- `Jwt:SigningKey` obligatorio (≥32 chars) con **`ValidateOnStart`**. `JWT_SIGNING_KEY` puede sobreescribir. Sin clave de respaldo.
- **FallbackPolicy** = `RequireAuthenticatedUser`; login y health con `AllowAnonymous`.
- **Rate limiting** fixed-window por IP en `POST /api/auth/login` → **429**.
- Recurso de otro tenant → **404** (filtro fail-closed).
- Matriz de roles del MVP:
  - **Recepción:** ficha y agenda; **no** lee HC.
  - **Profesional:** ficha, agenda, anamnesis, evoluciones (firma / anula / corrige solo las propias), HC. En el MVP puede abrir cualquier HC de su tenant; cada vista se audita.
  - **Admin:** ficha, catálogos, auditoría del tenant; **no** lee HC.
- `ClinicalAccessAudit` append-only en vista de HC y en altas/ediciones clínicas.
- Seed de usuarios demo (`*@demo.local`) solo en Development / `Seed:Enabled`. Contraseñas solo en `.env`.
- OpenAPI + Scalar en Development; FluentValidation → 400; `DomainException` → 422; exclusion de turnos → 409.

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| TenantId en header/query | Spoofing; viola ADR 0003. |
| Auth0 / Cognito | Overhead para Fase 1 portfolio. |
| 403 en cross-tenant | Filtra existencia de IDs ajenos. |
| Clave JWT hardcodeada | Arranque inseguro. |
| Recepción lee HC | Contrario a la matriz y a Ley 26.529 / 25.326 de diseño. |

## Consecuencias

- Tests cubren login, agenda vs HC por rol, vista auditada, anulación cruzada 422, cross-tenant 404, arranque sin JWT, 401 por FallbackPolicy y 429 en login.
- Los casos de uso viven en Application; la API solo adapta HTTP.
- Identity/JWT queda en Infrastructure; Application no referencia ASP.NET Identity.
