# ADR 0003 — Aislamiento de tenants fail-closed

**Estado:** Aceptado  
**Fecha:** 2026-10-22  
**Decisores:** Mario

---

## Contexto

El producto es multi-tenant con base compartida + `TenantId` (ADR 0001 / propuesta). Un filtro global “fail-open” (`!HasTenant || TenantId == …`) devolvería **todas** las filas clínicas cuando no hay tenant en el contexto. En software de historias clínicas eso es una fuga entre consultorios.

## Decisión

- Filtro **fail-closed**: `e.TenantId == _tenantContext.CurrentTenantIdOrEmpty`.
- Sin tenant ambient → `CurrentTenantIdOrEmpty` es `Guid.Empty` → **ninguna** fila `ITenantOwned` visible.
- Aplicar el filtro **una sola vez** recorriendo los tipos del modelo que implementan `ITenantOwned`.
- Seed, migraciones de datos y procesos de sistema que necesiten ver todos los tenants usan **`IgnoreQueryFilters()`** de forma explícita (y documentada).

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Fail-open (`!HasTenant \|\| …`) | Conveniente para seed, pero peligroso: olvido de `SetTenant` = leak de HC. |
| Exigir tenant siempre (excepción al consultar) | Más ruidoso; el filtro vacío ya bloquea lecturas sin romper inserts. |
| Filtros manuales por consulta | Fácil de olvidar; el global + `IgnoreQueryFilters` es más seguro. |

## Consecuencias

- Tests de aislamiento cubren: tenant A no ve B; sin tenant → vacío.
- El seeder demo llama `IgnoreQueryFilters()` cuando pregunta si ya hay datos.
- Relacionado con persistencia PostgreSQL / EF en ADR 0004.
