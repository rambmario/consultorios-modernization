# ADR 0006 — Frontend React + TypeScript (Vite)

**Estado:** Aceptado  
**Fecha:** 2026-10-24  
**Decisores:** Mario

---

## Contexto

Paso 5 de la Fase 1: hay que exponer login, ficha, agenda, HC y auditoría con la matriz de roles del ADR 0005, sin inflar el stack.

## Decisión

- **React 19 + TypeScript + Vite** en `/web`.
- Auth: login → JWT en **memoria + `sessionStorage`**; `fetch` agrega `Authorization: Bearer`; rutas protegidas por sesión y rol.
- Pantallas: login, pacientes (listado/alta/ficha), agenda, historia clínica (solo Profesional), auditoría (Admin / Profesional).
- **Modelos TypeScript a mano** (`web/src/api/models.ts`) alineados a los DTOs de Application. Sin cliente OpenAPI en Fase 1.
- Dev: proxy Vite `/api` → `http://localhost:5088`; CORS de Development ya permite `http://localhost:5173`.
- Tests: **Vitest** + Testing Library (matriz de roles, sesión, header Bearer, validación de login).
- UI en español; código y README de `/web` en inglés.

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Angular (como Pymen) | El stack objetivo de Consultorios ya era React/Vite. |
| Cliente OpenAPI generado | Contratos aún chicos; modelos a mano bastan. |
| `localStorage` para el JWT | Persistencia más larga de lo deseado; `sessionStorage` alcanza. |
| UI kit pesado | Suma dependencias sin beneficio en un MVP de portfolio. |

## Consecuencias

- Recepción no ve el enlace ni la ruta de HC. Admin no entra a agenda.
- Al cambiar DTOs hay que actualizar `models.ts`.
- El CI (Paso 6) debe correr `npm test` y `npm run build` en `/web`.
