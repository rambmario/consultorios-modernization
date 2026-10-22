# Propuesta de migración — Consultorios

**Fecha:** 2026-10-22  
**Entrada:** [legacy-analysis.md](./legacy-analysis.md), [owner-decisions.md](./owner-decisions.md)  
**Estrategia:** reescritura greenfield por slices verticales (ver ADR 0001)

El legado era **monousuario y de una sola clínica**. El producto nuevo es **multi-tenant por consultorio/clínica** (decisión de producto: que sirva a más de un profesional), con roles **Admin**, **Profesional** y **Recepción**.

No se migran datos reales. Seed 100 % ficticio (`Paciente Demo 1`, `ClinicaA`, `ProfesionalA`).

---

## 1. MVP (Fase 1 de producto)

### Qué construir

1. **Login obligatorio** (Identity + JWT) y roles Admin / Profesional / Recepción.
2. **Multi-tenant** por consultorio/clínica (`TenantId`, aislamiento fail-closed).
3. **Agenda de turnos** (desde cero; no existe en el legado).
4. **Ficha del paciente** con obra social (nombre + número de afiliado). Sin facturación.
5. **Anamnesis estructurada** (texto largo): MC, AEA, APP, AF, EF, AH — interpretación asumida en `owner-decisions.md`.
6. **Historia clínica** con **evoluciones firmadas** por el profesional (firmante obligatorio).
7. **Historial de diagnósticos** por paciente (no un único diagnóstico actual).
8. **Auditoría de accesos** a la historia clínica: quién vio o modificó qué, y cuándo.

### Por qué este MVP

- Es el núcleo clínico que el legado ya tenía (ficha + anamnesis + evoluciones + HC), más lo que la ley y el producto nuevo exigen (login, firmante, auditoría, tenancy) y lo que el dueño pidió crear (agenda).
- Deja fuera cloud (Fase 2), estudios/adjuntos (Fase 3) y recetas (Fase 4): la demo pública en AWS es prioridad de portfolio; estudios y prescripción no bloquean el MVP.
- Demuestra portfolio: Clean Architecture, React, PostgreSQL, multi-tenant, auditoría de un dato sensible.

### Fuera de MVP (explícito)

Pedidos de estudio y resultados, adjuntos, recetas, facturación / liquidación a obras sociales, geografía fina tipo catálogo de calles/barrios (se simplifica en la ficha). Copias de otros sistemas en la carpeta legacy: fuera de alcance.

---

## 2. Roadmap por fases

| Fase | Nombre | Entregable |
|------|--------|------------|
| **0** | Análisis | Este repo: análisis, decisiones del dueño, propuesta, ADR 0001 (actual) |
| **1** | Foundation + MVP clínico | Solución .NET 10 Clean Architecture, EF Core / PostgreSQL, tenancy, Identity/JWT, React (Vite), agenda, ficha, anamnesis, HC con evoluciones firmadas, historial de diagnósticos, auditoría, seed ficticio, CI |
| **2** | Cloud AWS | Demo pública: ECS Fargate **o** App Runner, RDS PostgreSQL, S3, SQS — a definir en ADR de cloud |
| **3** | Estudios y adjuntos | Pedidos de estudio, resultados, adjuntos en **S3** con control de acceso (nunca rutas locales) |
| **4** | Recetas | Prescripciones |
| **5** | Reportes | Reemplazo de los 3 Crystal (ficha, HC, pedido de estudio) por generación moderna (p. ej. QuestPDF) |

El orden de las fases 2–5 prioriza la demo pública en AWS. El MVP (Fase 1) no se infla.

---

## 3. Arquitectura propuesta

```
┌─────────────┐     HTTPS      ┌──────────────────────────────┐
│ React + TS  │ ──────────────►│  .NET 10 Web API             │
│ (Vite)      │     JWT        │  Auth + tenant middleware    │
└─────────────┘                └──────────────┬───────────────┘
                                              │
               ┌──────────────────────────────┼──────────────────────────────┐
               ▼                              ▼                              ▼
        Application                    Domain                         Infrastructure
      (casos de uso)              (entidades/reglas)            EF Core, Identity,
                                                                auditoría, (S3 en Fase 3)
                                              │
                                              ▼
                                    PostgreSQL (base compartida
                                    + TenantId)
```

| Decisión | Detalle |
|----------|---------|
| API | .NET 10 Web API, Clean Architecture |
| Front | React + TypeScript (Vite) |
| ORM / DB | EF Core + **PostgreSQL** |
| Multi-tenant | Base compartida + `TenantId` + filtro fail-closed (detalle en ADR posterior) |
| Auth | ASP.NET Identity + JWT; hash de passwords; roles Admin / Profesional / Recepción |
| Auditoría HC | Registro append-only de vista/alta/edición (quién, qué, cuándo, tenant) |
| Adjuntos | Fase 3: S3 + URLs firmadas / autorización; **nunca** path local |
| Cloud | AWS (ECS Fargate o App Runner, RDS PostgreSQL, S3, SQS) — **Fase 2** (demo pública) |
| Reportes | Fuera del MVP; Crystal no se porta |

### Multi-tenant vs legado

Legado: un EXE + una base local, un cardiólogo, una clínica.  
Nuevo: una app web, **muchos consultorios**, filas con `TenantId`. Un usuario no cruza de tenant.

### Matriz de roles (propuesta de diseño del MVP)

| Capacidad | Admin | Profesional | Recepción |
|-----------|:-----:|:-----------:|:---------:|
| Usuarios y roles del tenant | Sí | No | No |
| Catálogos (obra social, diagnósticos) | Sí | Lectura / uso | Lectura / uso |
| Agenda | No (salvo que también sea Profesional) | Sí | Sí |
| Ficha demográfica + OS | Sí (administrativa) | Sí | Sí |
| Anamnesis, evoluciones, HC, diagnósticos clínicos | **No** | Sí (evoluciones: solo como firmante) | **No** |
| Ver auditoría | Sí | La propia / del paciente que atiende (a confirmar en implementación) | No |

Recepción **no lee** la historia clínica. Eso alinea el diseño con Ley 26.529 (acceso por personal autorizado al acto clínico) y 25.326 (dato sensible). Si el dueño quiere que Recepción vea la HC, hay que reabrir esta celda de forma explícita.

---

## 4. Mapeo legado → modelo nuevo

Nombres en inglés en código; conceptos clínicos en español en docs.

| Legado | Nuevo (propuesta) | Notas |
|--------|-------------------|-------|
| *(no existe)* | `Tenant` | Consultorio/clínica |
| *(no existe en este árbol)* | `ApplicationUser` + roles | Login obligatorio; hash |
| `PACIENTE` | `Patient` | Sin copiar filas reales |
| `OBRA_SOCIAL` + `afiliado_os` | `HealthPlan` + `PatientCoverage` | Nombre + nro. de afiliado |
| `MC`/`AEA`/`APP`/`AF`/`EF`/`AH` | `Anamnesis` (secciones) | Texto largo; interpretación asumida |
| `EVOLUCION` | `ProgressNote` | Texto largo; **ProfessionalId** + firma (quién, cuándo) |
| `DIAGNOSTICO` (1 en ficha) | `Diagnosis` + `PatientDiagnosis` | Historial (fecha, profesional, vigencia) |
| HC calculada (`sp_Generar_HC`) | Vista/proyección de anamnesis + notas + diagnósticos (+ estudios en Fase 3) | Se reimplementa; no se portan SP |
| *(no existe)* | `Appointment` | Agenda desde cero |
| *(no existe)* | `ClinicalAccessAudit` | Quién vio/modificó HC |
| `ESTUDIO` / `ANALISIS` / `MOVIMIENTO_ESTUDIO` / `DETALLE_ESTUDIO` | Fase 3 (`StudyOrder`, …) | |
| `ECODOPLER` (path local) | Fase 3 `Attachment` en S3 | |
| Recetas | Fase 4 | |
| Provincia/localidad/calle/barrio | Dirección simple en `Patient` | Sin catálogo de calles en el MVP |
| Crystal `.rpt` | Fuera de MVP | |

---

## 5. Ley 26.529 y Ley 25.326 (diseño)

No es asesoramiento legal. Son **restricciones de diseño** del producto.

**Ley 26.529 (derechos del paciente / HC)**

- La HC es un documento clínico: el MVP la trata como agregado auditado, no como texto suelto.
- Cada evolución lleva **identificación del profesional** (decisión 14).
- El paciente tiene derecho a acceder a su HC: se deja el caso de uso para una fase posterior (portal o exportación); el modelo no debe impedir una exportación íntegra.
- Integridad: no hay delete físico de evoluciones en el MVP (anulación / corrección con traza), a precisar en dominio.
- Confidencialidad: Recepción no lee la HC; todo acceso clínico se audita.

**Ley 25.326 (datos personales)**

- Los datos de salud son **sensibles**. Acceso por rol + tenant; seeds ficticios; nada de datos reales en el repo.
- Minimización: la ficha MVP pide lo necesario para atender (identidad, contacto, OS, anamnesis, diagnósticos), no un padrón extra.
- Seguridad: TLS, passwords hasheados, secretos fuera del repo, adjuntos (Fase 3) con autorización.
- Residencia / transferencia al usar AWS: se resuelve en el ADR de cloud (Fase 2).

---

## 6. Riesgos de la migración

| Riesgo | Mitigación |
|--------|------------|
| El legado no tiene agenda ni tenancy: se inventa mal el flujo | Slice chico de agenda (alta / cancelar / listar día); preguntas nuevas al dueño si aparece duda, no adivinar |
| Interpretación asumida de siglas de anamnesis | Queda marcada como tal; la UI muestra el nombre largo, no solo la sigla |
| Sin cuerpos de SP | Reimplementar HC en dominio; no intentar paridad SQL |
| Recepción vs HC | Matriz de arriba; test de autorización |
| Fuga de PII al documentar o sembrar | Reglas de `AGENTS.md` / `registro.mdc`; seed demo evidente |
| Alcance inflado (estudios, recetas, Crystal) | Roadmap; el MVP no los incluye |
| Cloud AWS indefinido | No bloquear el MVP; ADR de cloud en Fase 2 (antes de estudios) |

---

## 7. Criterios de éxito del MVP

- [ ] Un usuario de `ClinicaA` inicia sesión; sin token no hay API.
- [ ] Admin de `ClinicaA` no ve datos de `ClinicaB`.
- [ ] Recepción agenda un turno para `Paciente Demo 1` y **no** puede abrir la HC.
- [ ] Profesional crea/edita ficha, anamnesis (6 secciones) y una evolución **firmada**; otro profesional del mismo tenant queda registrado si mira esa HC.
- [ ] Se puede cargar más de un diagnóstico en el tiempo sobre el mismo paciente.
- [ ] La auditoría lista quién vio o modificó la HC y cuándo.
- [ ] Seed 100 % ficticio; CI verde.

---

## 8. Próximo paso de implementación

Fase 1 / Paso 1: solución .NET 10 + proyectos de test.
