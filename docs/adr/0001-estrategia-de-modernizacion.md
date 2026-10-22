# ADR 0001 — Estrategia de modernización (greenfield por slices)

**Estado:** Aceptado  
**Fecha:** 2026-10-22  
**Decisores:** Mario (owner del portfolio)

---

## Contexto

Consultorios es un WinForms .NET 2.0 + SQL Server Express, ~14k LOC clínicos, 14 tablas, sin login en este árbol, sin agenda y sin auditoría. Se usó en una sola clínica, un solo cardiólogo. No se van a migrar datos reales.

El producto nuevo no es un calco del EXE: es un SaaS **multi-tenant** (Admin / Profesional / Recepción) con agenda, firma de evoluciones y auditoría de la historia clínica, para que sirva a más de un profesional.

El proyecto es de **portfolio**: necesita entregables visibles por fases, documentación pública y un MVP creíble, no paridad 1:1 con WinForms.

## Decisión

Adoptar una **reescritura greenfield por slices verticales**:

1. Documentar el legado (Fase 0) como referencia de solo lectura; no modificarlo.
2. Levantar el stack moderno (.NET 10 Web API Clean Architecture + React/TypeScript + PostgreSQL) en paralelo.
3. Entregar primero el **MVP clínico** (login, tenancy, agenda, ficha, anamnesis, HC firmada, diagnósticos, auditoría).
4. Publicar la **demo en AWS (Fase 2)** — prioridad de portfolio — antes de sumar módulos clínicos extra.
5. Sumar estudios/adjuntos (S3) en **Fase 3** y recetas en **Fase 4**.
6. No portar stored procedures ni Crystal; la HC se reimplementa en dominio. El detalle de ECS Fargate vs App Runner se decide en el ADR de cloud de Fase 2.

No se “estrangula” un sistema en producción: no hay datos a convivir ni un servicio legado que seguir llamando.

## Alternativas consideradas

| Alternativa | Por qué se descartó |
|-------------|---------------------|
| Strangler fig clásico (fachada frente al WinForms/SQL vivo) | No hay instalación productiva a convivir ni migración de datos |
| Big bang de todas las fases (estudios, recetas, AWS, reportes) | Meses sin demo; infla el MVP |
| Lift-and-shift a .NET 4.8 / WinForms | No moderniza UX ni sirve como portfolio web/cloud |
| API encima de los SP y la base `Maestro_consultorio` | Congela deuda SQL, no hay cuerpos de SP versionados, impide tenancy limpio y PostgreSQL |
| WPF / MAUI | Sigue siendo escritorio; no muestra Web API + React + AWS |

## Consecuencias

**Positivas**

- Demo usable al cerrar el MVP; la publicación en AWS (Fase 2) no espera estudios ni recetas.
- Dominio nuevo alineado a ley y a multi-tenant, no al EXE monousuario.
- Menos presión por paridad visual con WinForms.

**Negativas / trade-offs**

- No hay paridad automática con `sp_Generar_HC`; hay que diseñar la HC de nuevo.
- La agenda se inventa (no hay legado que copiar): hay que acotar el slice.
- Durante Fase 1 no hay estudios ni recetas (aceptable: lo pidió el dueño).

**Seguimiento**

- Alcance y roadmap: `docs/legacy/migration-proposal.md`.
- ADR posteriores: aislamiento de tenants, auth/roles, persistencia PostgreSQL, cloud AWS.
