# ADR 0007 — CI con GitHub Actions

**Estado:** Aceptado  
**Fecha:** 2026-10-24  
**Decisores:** Mario

---

## Contexto

Paso 6 de la Fase 1: hay API .NET 10 con Testcontainers (PostgreSQL) y SPA React/Vite. Hace falta un pipeline reproducible en cada push/PR, sin secretos reales en el repo, con 0 tests skippeados.

## Decisión

- Workflow `.github/workflows/ci.yml` en **push a `main`** y **pull_request**.
- Job **backend**: `setup-dotnet` desde `global.json`, restore, build **Release** (warnings as errors vía `Directory.Build.props`), `dotnet test` con Testcontainers (Docker en `ubuntu-latest`), artifact de TRX + cobertura Coverlet.
- Job **frontend**: `setup-node` 22 con cache npm, `npm ci`, lint si existe script, Vitest, build de producción.
- Credenciales de test (**JWT** y `DEMO_PASSWORD_*`) **generadas o fijas de demo en el job**; no hay secrets de producción en el workflow. PostgreSQL de integración lo levanta Testcontainers.
- **Dependabot** semanal para NuGet, npm (`/web`) y GitHub Actions.
- Badge de CI en el README.

## Alternativas consideradas

| Alternativa | Por qué no |
|-------------|------------|
| Azure Pipelines | Overhead extra para portfolio; GitHub Actions alcanza. |
| Secretos de repo para JWT/Postgres de tests | Innecesario; JWT se genera efímero y Testcontainers trae su propio Postgres. |
| Un solo job monolítico | Fallas de front/back menos claras; paralelismo perdido. |

## Consecuencias

- Los tests de integración corren en CI (Docker disponible); no se skippea ninguno.
- Cache de NuGet y npm acelera runs posteriores.
- Dependabot abrirá PRs semanales; conviene revisarlos antes de merge.
- El badge apunta a GitHub Actions cuando el repo esté publicado.
