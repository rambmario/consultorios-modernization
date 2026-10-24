# ADR 0006 — Frontend React + TypeScript (Vite)

**Estado:** Aceptado  
**Fecha:** 2026-10-24  
**Decisores:** Mario

---

## Contexto

Paso 5 de la Fase 1: hay que exponer login, ficha, agenda, HC y auditoría con la matriz de roles del ADR 0005. El objetivo del portfolio es mostrar un stack de mercado, no un MVP de `fetch` + HTML plano.

## Decisión

- **React 19 + TypeScript + Vite** en `/web`.
- **TanStack Query** para todas las llamadas a la API: cache, estados de loading/error y **invalidación al guardar** (alta de paciente, firma de evolución, alta de turno).
- **MUI** como librería de UI, con **tema propio** (paleta clínica crema/verde, tipografía Georgia + Segoe UI) y componentes accesibles (labels, tabs, tablas, alertas).
- Auth: login → JWT en **memoria + `sessionStorage`**; `fetch` agrega `Authorization: Bearer`; rutas protegidas por sesión y rol.
- Pantallas: login, pacientes (listado/alta/ficha con pestañas según rol), agenda, historia clínica (solo Profesional), auditoría (Admin / Profesional).
- **Fechas en `es-AR`**: `dd/MM/yyyy HH:mm` (America/Argentina/Buenos_Aires) en toda la UI.
- **Anamnesis**: nombres largos visibles (Motivo de consulta, Antecedentes de la enfermedad actual, etc.), no solo siglas.
- **Modelos TypeScript a mano** (`web/src/api/models.ts`) alineados a los DTOs de Application. Sin cliente OpenAPI en Fase 1.
- Dev: proxy Vite `/api` → `http://localhost:5088`; CORS de Development ya permite `http://localhost:5173`.
- Tests: **Vitest** + Testing Library (matriz de roles, sesión, header Bearer, lista de pacientes, ficha por rol, HC oculta a Recepción, firmar evolución).
- UI en español; código y README de `/web` en inglés.

Se agregan TanStack Query y MUI porque son **estándar de mercado** y parte explícita del objetivo del portfolio (no un extra opcional del MVP).

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Angular (como Pymen) | El stack objetivo de Consultorios ya era React/Vite. |
| Cliente OpenAPI generado | Contratos aún chicos; modelos a mano bastan. |
| `localStorage` para el JWT | Persistencia más larga de lo deseado; `sessionStorage` alcanza. |
| `fetch` + `useEffect` sin cache | No cubre invalidación ni estados de forma uniforme; no es el stack de portfolio. |
| CSS propio sin UI kit | Más trabajo de a11y y menos representativo del mercado. |
| Otra librería (Chakra, Ant) | MUI es la más habitual en React de producto. |

## Consecuencias

- Recepción no ve el tab ni la ruta de HC. Admin no entra a agenda.
- Al cambiar DTOs hay que actualizar `models.ts`.
- El CI (Paso 6) debe correr `npm test` y `npm run build` en `/web`.
- El tema MUI queda en `web/src/theme/consultoriosTheme.ts`; las queries en `web/src/api/hooks.ts`.
