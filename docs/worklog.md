# Worklog — Consultorios Modernization

Registro cronológico de sesiones de trabajo.

---

## 2026-10-22 — Fase 0 / Paso 1: estructura y registro

**Hora (aprox.):** 19:35–19:50 (UTC-3, America/Buenos_Aires)

### Objetivo de la sesión

Inicializar el repo `consultorios-modernization` con la misma disciplina de registro que `pymen-modernization`, más reglas de privacidad de software de salud. Sin análisis del legacy y sin código de la aplicación nueva.

### Qué se hizo

**Creados:**

- `README.md` (borrador en inglés: contexto, privacidad, stack objetivo, layout)
- `CHANGELOG.md`
- `.gitignore` (Visual Studio + Node/Vite + `.env` + `*.bak` / `*.mdf` / `*.ldf`)
- `AGENTS.md`
- `.cursor/rules/registro.mdc` (registro + privacidad)
- `docs/worklog.md`, `docs/ai-usage.md`
- `docs/adr/README.md` (formato de ADRs; todavía no hay decisiones)
- `git init` en el repo nuevo

**Ya existía (no modificado en contenido):** `commands/revisar-paso.md`

**No se tocó:** `D:\VBNetAnterior\Consultorios` (solo lectura).

### Decisiones tomadas y por qué

- Reutilizar el flujo de portfolio de Pymen (worklog, ai-usage, ADRs, CHANGELOG, un commit por tarea, docs en español / README y commits en inglés) para mantener el mismo estándar entre proyectos.
- Poner las reglas de privacidad **por encima** del resto en `AGENTS.md` y `registro.mdc`: es software de historias clínicas.
- Dejar `docs/adr/` preparado pero vacío de ADRs: la primera decisión de arquitectura corresponde al Paso 3.
- No crear aún `docs/legacy/*`: ese material es el Paso 2 y el Paso 3.

### Problemas encontrados y cómo se resolvieron

- El directorio no era un repo git: se inicializó en local, sin remoto ni push.

### Próximo paso

Paso 2 — análisis del legacy en `docs/legacy/legacy-analysis.md` y preguntas abiertas en `docs/legacy/owner-decisions.md`. Esperar OK.

---

## 2026-10-22 — chore: comando Cursor y Author

**Hora (aprox.):** 19:40 (UTC-3, America/Buenos_Aires)

### Objetivo

Ajustes pedidos al OK del Paso 1, en un commit `chore`.

### Qué se hizo

- Movido `commands/revisar-paso.md` → `.cursor/commands/revisar-paso.md`.
- Checklist adaptada: punto de datos clínicos, alcance según `docs/legacy/migration-proposal.md`, N/A de tests/build mientras no haya código, legacy path explícito.
- README Author: Mario Ramb — Senior .NET Full Stack Developer + LinkedIn/GitHub (igual que Pymen).

### Próximo paso

Paso 2 — análisis del legacy (solo lectura).

---

## 2026-10-22 — Fase 0 / Paso 2: análisis del legacy

**Hora (aprox.):** 19:40–20:20 (UTC-3, America/Buenos_Aires)

### Objetivo de la sesión

Documentar el sistema legacy Consultorios (solo lectura) y dejar preguntas abiertas para el dueño. Sin código de la app nueva y sin copiar datos clínicos.

### Qué se hizo

**Creados:**

- `docs/legacy/legacy-analysis.md` (stack, métricas, módulos, flujos, modelo, seguridad, riesgos)
- `docs/legacy/owner-decisions.md` (16 preguntas; ninguna decisión cerrada)

**Analizado (solo lectura):** `D:\VBNetAnterior\Consultorios` — `Consultorios.vbproj` / `.sln`, `frmInicio`, `cls*`, `Conexion`, `Consultas/CREAR_TABLAS.sql`, `Consultas/GENERAR HC.sql`, reportes en `Reporte/`. No se abrieron `.mdf` / `.ldf`.

### Decisiones tomadas y por qué

- Inventariar solo el proyecto compilado clínico; las copias de otros sistemas en la carpeta quedan fuera de alcance, sin detallarlas.
- No adivinar agenda/recetas/facturación: constatar que no existen en código y preguntar al dueño.
- Describir columnas y FKs, nunca filas ni nombres reales.

### Problemas encontrados y cómo se resolvieron

- Drift de esquema entre el script de creación y las `cls*`: se documentó y se preguntó cuál es la BD real.
- El árbol incluye copias de otros sistemas en la carpeta, fuera de alcance: se excluyeron de las métricas clínicas.
- Cuerpos de SP casi ausentes: se usó el borrador de `GENERAR HC` y la lista de nombres referenciados.

### Próximo paso

Paso 3 — propuesta de migración + ADR 0001. Esperar OK. No escribir código.

---

## 2026-10-22 — Fase 0 / Paso 2b: decisiones del dueño

**Hora (aprox.):** 19:51–20:05 (UTC-3, America/Buenos_Aires)

### Objetivo

Registrar las 16 respuestas del dueño, sanitizar documentos públicos y dejar el análisis alineado.

### Qué se hizo

- `docs/legacy/owner-decisions.md`: 16 decisiones cerradas (2026-10-22); preguntas originales vaciadas.
- `docs/legacy/legacy-analysis.md`: esquema `cls*`, anamnesis con interpretación asumida, login histórico, copias de otros sistemas en la carpeta fuera de alcance (sin nombrarlas).
- `docs/ai-usage.md`: quitado el link a un id interno de chat.

### Decisiones tomadas y por qué

Las 16 las aportó Mario; la IA solo documentó y saneó menciones.

### Próximo paso

Paso 3 — `migration-proposal.md` + ADR 0001.

---

## 2026-10-22 — Fase 0 / Paso 3: propuesta y ADR 0001

**Hora (aprox.):** 20:05–20:20 (UTC-3, America/Buenos_Aires)

### Objetivo

Propuesta de migración y ADR de estrategia, sin código de la app.

### Qué se hizo

- `docs/legacy/migration-proposal.md`: MVP, fases, stack, mapeo, roles, leyes 26.529 / 25.326 a nivel diseño, criterios de éxito.
- `docs/adr/0001-estrategia-de-modernizacion.md`: greenfield por slices verticales.
- README de estado y `docs/adr/README.md` actualizados.

### Decisiones tomadas y por qué

- Greenfield (no strangler clásico): no hay sistema productivo a convivir ni migración de datos.
- Multi-tenant y roles aunque el legado fuera monousuario: decisión de producto del dueño.
- Recepción no lee la HC en el MVP (diseño; se puede reabrir).

### Próximo paso

Esperar OK. Siguiente implementación: Fase 1 foundation.

---

## 2026-10-22 — Ajuste de roadmap: AWS en Fase 2

**Hora (aprox.):** 19:57–20:05 (UTC-3, America/Buenos_Aires)

### Objetivo

Mover Cloud AWS a Fase 2 (prioridad de demo pública) y reordenar el resto.

### Qué se hizo

- `docs/legacy/migration-proposal.md` y ADR 0001: orden 1 MVP → 2 Cloud AWS → 3 Estudios/S3 → 4 Recetas → 5 Reportes.

### Decisiones

- AWS antes de estudios porque la demo pública es prioridad de portfolio.

### Próximo paso

Fase 1 / Paso 1: solución .NET 10.

---

## 2026-10-22 — Fase 1 / Paso 1: solución .NET 10

**Hora (aprox.):** 19:57–20:10 (UTC-3, America/Buenos_Aires)

### Objetivo

Crear la solución Clean Architecture (.NET 10) con proyectos, props compartidos, tests smoke y ADR de estructura.

### Qué se hizo

- `Consultorios.sln` + `src/Consultorios.{Domain,Application,Infrastructure,Api}` + `tests/Consultorios.{Domain.Tests,Application.Tests,Api.IntegrationTests}`.
- `Directory.Build.props`, `Directory.Packages.props` (CPM), `.editorconfig`, `global.json`, `nuget.config`, `.env.example`.
- `Entity` (Guid v7), `DomainException`, `ITenantOwned`; `TimeProvider` en Infrastructure; `/health`.
- ADR 0002.
- `dotnet build -c Release` 0 warnings; **4 tests passed, 0 failed, 0 skipped**.

### Decisiones

- `.sln` clásico (no `.slnx`).
- CPM desde el inicio (lección Pymen / Dependabot).
- NuGet solo nuget.org.

### Problemas

- Ninguno bloqueante.

### Próximo paso

Paso 2 — dominio clínico. Esperar OK.

---

## 2026-10-22 — Fase 1 / Paso 2: dominio

**Hora (aprox.):** 20:58–21:20 (UTC-3, America/Buenos_Aires)

### Objetivo

Implementar el dominio clínico con las reglas pedidas y tests unitarios.

### Qué se hizo

- Entidades: `Tenant`, `Patient` + `Address`, `HealthPlan` + `PatientCoverage`, `Anamnesis` (6 secciones con nombre largo), `ProgressNote` (firma, anulación, corrección), `Diagnosis` + `PatientDiagnosis` (vigencia), `Appointment` (estados + sin solape), `ClinicalAccessAudit` (append-only).
- Tests de dominio: 32 (más 2 smoke de solution). Suite total **34 passed, 0 failed, 0 skipped**.

### Decisiones

- Evolución se firma al crear; no hay mutador de `Body`. Corrección deja la original `Superseded` y crea una nueva firmada con `CorrectsNoteId`.
- El solape de agenda solo considera turnos `Scheduled`/`Confirmed` del mismo profesional; intervalos adyacentes no solapan.
- Un diagnóstico no puede estar vigente dos veces a la vez; se puede reabrir en una fila nueva después de `Close`.
- Auditoría: solo factory `Record`, sin métodos de instancia públicos.

### Problemas encontrados y cómo se resolvieron

- `.gitignore` tiene `coverage/` (reportes de tests). En Windows ignoró `Domain/Coverage`. Se movió a `HealthPlans/` y se commitea aparte.

### Próximo paso

Paso 3 — persistencia EF Core + PostgreSQL. Esperar OK.

---

## 2026-10-22 — fix(domain): firmante y Guard

**Hora (aprox.):** 21:40 (UTC-3, America/Buenos_Aires)

### Objetivo

Ajustes pedidos al OK del Paso 2, antes de persistencia.

### Qué se hizo

- `ProgressNote.Cancel` / `Correct` solo el profesional que firmó; otro → `DomainException`. Test `Another_professional_cannot_cancel_or_correct`.
- `Patient.EnsureTenant` movido a `Domain/Common/Guard` (`AgainstEmpty` / `AgainstEmptyTenant`); todas las entidades `ITenantOwned` lo usan.
- `Address`: ctor privado + setters privados para EF.
- Dominio: **33 passed, 0 failed, 0 skipped**.

### Decisiones

- El firmante queda atado a `ProfessionalId` de la nota; no hay rol “supervisor” que anule en el MVP.

### Próximo paso

Paso 3 — persistencia EF Core + PostgreSQL, exclusion constraint de turnos, Testcontainers.

---

## 2026-10-22 — Fase 1 / Paso 3: persistencia

**Hora (aprox.):** 21:45–22:00 (UTC-3, America/Buenos_Aires)

### Objetivo

Persistencia EF Core + PostgreSQL, filtro fail-closed, exclusion de turnos y tests con Testcontainers.

### Qué se hizo

- `ConsultoriosDbContext` + Fluent API; FKs `Restrict`; filtro `ITenantOwned` fail-closed.
- `IConsultoriosDbContext` / `ITenantContext` en Application; `AmbientTenantContext`.
- Migración `InitialCreate` + `btree_gist` + `EX_Appointments_ProfessionalOverlap`.
- `docker-compose` Postgres 16; `DemoDataSeeder` ficticio; `/health` y `/health/ready`.
- Tests: aislamiento de tenant, round-trip de `Address`, solape paralelo (23P01) y reuso de hueco cancelado.
- ADR 0003 y ADR 0004.

### Decisiones

- Exclusion en SQL (EF no modela `EXCLUDE`); `WHERE Status IN (0, 1)`.
- Testcontainers sin skip: si no hay Docker, el test falla.
- `ProfessionalId` queda como Guid opaco hasta Identity (Paso 4).

### Problemas

- `PostgreSqlBuilder()` parameterless está obsolete (warning-as-error). Se usa el ctor con imagen.
- Las migraciones de EF usan namespace en bloque: `.editorconfig` silencia `IDE0161` en esa carpeta.

### Próximo paso

`/revisar-paso` y esperar OK. Paso 4 — API + Identity/JWT.

---

## 2026-10-24 — test: enteros de AppointmentStatus

**Hora (aprox.):** 11:05 (UTC-3, America/Buenos_Aires)

### Objetivo

Antes del Paso 4, fijar los valores numéricos que usa la exclusion constraint.

### Qué se hizo

- Test `Occupying_statuses_keep_exclusion_constraint_integers` (`Scheduled = 0`, `Confirmed = 1`).
- Comentario en `AppointmentStatus` y en `InitialCreate` (cambiar esos valores rompe `EX_Appointments_ProfessionalOverlap`).

### Próximo paso

Paso 4 — API + Identity/JWT + auditoría.

---

## 2026-10-24 — Fase 1 / Paso 4: API + Identity/JWT + auditoría

**Hora (aprox.):** 11:10–12:10 (UTC-3, America/Buenos_Aires)

### Objetivo

Exponer el MVP clínico por HTTP con login, roles y auditoría de HC.

### Qué se hizo

- Identity (`ApplicationUser` Guid + `TenantId`) + JWT + middleware de tenant/usuario.
- Endpoints: auth, pacientes, agenda, catálogos, HC / anamnesis / evoluciones / diagnósticos, auditoría.
- Seed de usuarios `*@demo.local` (contraseñas solo en `.env`).
- Tests: login, recepción vs HC (403), vista auditada, firmante, cross-tenant 404, admin sin HC, JWT corto, probe 401, 429.
- ADR 0005.

**Conteo exacto de tests (Paso 4, `dotnet test -c Release`):**

| Proyecto | Passed | Failed | Skipped |
|----------|-------:|-------:|--------:|
| `Consultorios.Domain.Tests` | 34 | 0 | 0 |
| `Consultorios.Application.Tests` | 3 | 0 | 0 |
| `Consultorios.Api.IntegrationTests` | 14 | 0 | 0 |
| **Total** | **51** | **0** | **0** |

### Decisiones

- Recepción no lee HC. Admin tampoco. Profesional del tenant puede abrir HC; solo el firmante anula/corrige.
- Cross-tenant → 404. Exclusion de turnos → 409. `DomainException` → 422.
- `ApplicationUser` no implementa `ITenantOwned` (login sin tenant ambient).

### Problemas

- `FrameworkReference` Microsoft.AspNetCore.App hizo redundantes paquetes de Extensions (NU1510). Se quitaron.

### Próximo paso

`/revisar-paso` y esperar OK. Paso 5 — React/Vite.

---

## 2026-10-24 — Fase 1 / Paso 5: frontend React

**Hora (aprox.):** 11:25–12:10 (UTC-3, America/Buenos_Aires)

### Objetivo

SPA React/Vite contra la API del Paso 4, con matriz de roles y tests.

### Qué se hizo

- `web/`: Vite, React 19, TypeScript, React Router.
- Login (`sessionStorage` + Bearer), ficha, agenda, HC (solo Profesional), auditoría.
- Tests Vitest: roles, sesión, header JWT, validación de login. **6 passed, 0 failed, 0 skipped.**
- ADR 0006.
- El worklog del Paso 4 ya tenía el conteo 51/0/0 (commit `docs` previo).

### Decisiones

- Modelos TS a mano. Proxy `/api` → `localhost:5088`. JWT en `sessionStorage`, no `localStorage`.
- UI en español; sin UI kit.

### Próximo paso

Ajustes del Paso 5 (Query, MUI, fechas, tests) y luego CI.

---

## 2026-10-24 — Fase 1 / Paso 5 (ajustes): TanStack Query + MUI + tests

**Hora (aprox.):** 11:56–12:15 (UTC-3, America/Buenos_Aires)

### Objetivo

Completar el Paso 5 con el stack de portfolio (Query + MUI), fechas es-AR, anamnesis con nombres largos y ≥15 tests de front, con conteo exacto.

### Qué se hizo

- `@tanstack/react-query` para login, pacientes, ficha, HC, evoluciones, agenda y auditoría; invalidación al guardar.
- MUI + tema propio (`consultoriosTheme`): AppBar, tabs, tablas, formularios, alertas.
- Fechas `dd/MM/yyyy HH:mm` (`es-AR`, America/Argentina/Buenos_Aires).
- Anamnesis: nombres largos visibles (no siglas).
- Ficha con pestañas según rol; HC oculta para Recepción y Admin.
- ADR 0006 actualizado (Query y MUI como estándar de mercado / portfolio).

**Conteo exacto de tests de front (`npm test` en `/web`):**

| Archivo | Passed | Failed | Skipped |
|---------|-------:|-------:|--------:|
| `auth/roles.test.ts` | 3 | 0 | 0 |
| `auth/session.test.ts` | 1 | 0 | 0 |
| `api/client.test.ts` | 1 | 0 | 0 |
| `api/hooks.test.tsx` | 2 | 0 | 0 |
| `pages/LoginPage.test.tsx` | 1 | 0 | 0 |
| `i18n/dates.test.ts` | 2 | 0 | 0 |
| `clinical/anamnesisSections.test.ts` | 1 | 0 | 0 |
| `clinical/ClinicalRecordPanel.test.tsx` | 3 | 0 | 0 |
| `patients/PatientList.test.tsx` | 3 | 0 | 0 |
| `patients/PatientFile.test.tsx` | 3 | 0 | 0 |
| **Total** | **20** | **0** | **0** |

### Decisiones

- Query y MUI no son opcionales: son el stack que el portfolio debe mostrar.
- La HC se pide recién al abrir la pestaña (la vista queda auditada).

### Problemas

- MUI 7: `InputLabelProps` pasó a `slotProps.inputLabel`; `Stack` no acepta `flexWrap` suelto.
- Testing Library: el label de TextField required incluye asterisco; el texto de “Examen físico” chocaba con el valor de demo.

### Próximo paso

Paso 6 — CI (GitHub Actions + Dependabot).

---

## 2026-10-24 — Fase 1 / Paso 6: CI

**Hora (aprox.):** 12:15–12:25 (UTC-3, America/Buenos_Aires)

### Objetivo

Pipeline reproducible en push/PR, sin secretos reales, 0 skip.

### Qué se hizo

- `.github/workflows/ci.yml`: jobs paralelos Backend (restore/build Release/`dotnet test` + Coverlet + artifact) y Frontend (`npm ci`, lint si hay, Vitest, build).
- JWT efímero y `DEMO_PASSWORD_*` de demo escritos en `.env` del job. Postgres lo levanta Testcontainers.
- `.github/dependabot.yml`: NuGet, npm `/web`, GitHub Actions (semanal).
- ADR 0007. Badge de CI en el README.

**Conteo de tests (último conocido, no se re-ejecutó el backend en este paso):**

| Suite | Passed | Failed | Skipped |
|-------|-------:|-------:|--------:|
| Backend (`dotnet test -c Release`, Paso 4) | 51 | 0 | 0 |
| Frontend (`npm test`, Paso 5 ajustes) | 20 | 0 | 0 |

### Decisiones

- Mismo esquema que Pymen (ADR 0008 allá): dos jobs, credenciales efímeras, sin skip.
- Dependabot agrupa React, MUI, TanStack y tooling.

### Problemas

- El repo todavía no tiene remote; el badge apunta a `rambmario/consultorios-modernization` para cuando se publique.

### Próximo paso

`/revisar-paso` y esperar OK. Paso 7 — README y capturas (solo datos ficticios).
