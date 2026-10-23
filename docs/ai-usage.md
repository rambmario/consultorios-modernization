# AI usage log — Consultorios Modernization

Registro de asistencia de IA (transparencia de portfolio).

---

## 2026-10-22 — Fase 0 / Paso 1

### Pedido principal

Crear el repo `consultorios-modernization` con README, CHANGELOG, `.gitignore`, `AGENTS.md`, regla Cursor de registro (mismas convenciones que `pymen-modernization`) y reglas de privacidad de software de salud. Worklog, ai-usage y carpeta de ADRs. Un commit, sin push. No escribir código de la app. No analizar el legacy todavía (Pasos 2 y 3 quedan a la espera de OK).

### Qué generó la IA

- Scaffolding del repositorio y archivos de registro listados en el worklog.
- Adaptación de las reglas de Pymen al dominio clínico (privacidad, rutas legacy/nuevo, idiomas, Conventional Commits).
- `git init` y el primer commit local.

### Qué revisó o debe revisar el desarrollador

- Que las reglas de privacidad cubran el caso de uso (historias clínicas) y no falte ninguna categoría de dato sensible.
- Que no se haya copiado nada desde `D:\VBNetAnterior\Consultorios`.
- OK para pasar al Paso 2 (análisis del legacy, solo lectura).

### Tiempo estimado ahorrado

~30–45 min de armado de scaffolding, `.gitignore` y reglas de registro/privacidad.

---

## 2026-10-22 — chore post Paso 1

### Pedido principal

Mover el comando de revisión a `.cursor/commands/`, adaptar la checklist al repo clínico, y alinear la sección Author del README con Pymen.

### Qué generó la IA

- Nuevo `.cursor/commands/revisar-paso.md`, README Author, entradas de registro, commit `chore`.

### Qué revisó o debe revisar el desarrollador

- Que el comando Cursor se dispare desde `.cursor/commands/revisar-paso.md`.

### Tiempo estimado ahorrado

~10 min.

---

## 2026-10-22 — Fase 0 / Paso 2

### Pedido principal

Analizar el legacy Consultorios (solo lectura) y producir `docs/legacy/legacy-analysis.md` + `docs/legacy/owner-decisions.md`. Privacidad clínica estricta. Al terminar, correr `/revisar-paso` y esperar OK. Sin código de la app.

### Qué generó la IA

- Inventario de stack, LOC, formularios, reportes y SPs.
- Flujos de paciente / HC / evolución / estudios.
- Modelo ER (14 tablas) y huecos de seguridad.
- 16 preguntas abiertas (sin cerrar ninguna).
- Exploración asistida del root compilado, cotejada contra `cls*`, `frmInicio` y scripts de `Consultas/`.

### Qué revisó o debe revisar el desarrollador

- Exactitud de módulos ausentes (agenda, recetas, login) frente al uso real.
- Significado de las siglas de anamnesis y el drift de esquema.
- Que ningún dato real haya quedado en los docs.
- OK para pasar al Paso 3 (propuesta + ADR).

### Tiempo estimado ahorrado

~3–5 h de lectura de formularios, clases de datos y scripts SQL.

---

## 2026-10-22 — decisiones del dueño

### Pedido principal

Pasar las 16 respuestas a decisiones cerradas, sanitizar el análisis (no nombrar copias ajenas), quitar el link interno de chat en `ai-usage.md`, y luego seguir con el Paso 3 en un commit aparte.

### Qué generó la IA

- Actualización de `owner-decisions.md` y alineación de `legacy-analysis.md` / worklog / CHANGELOG.
- Eliminación del id interno de chat.

### Qué revisó o corrigió el desarrollador

- El contenido de las 16 decisiones lo aportó Mario.

### Tiempo estimado ahorrado

~20–30 min de redacción cruzada.

---

## 2026-10-22 — Fase 0 / Paso 3

### Pedido principal

Escribir `migration-proposal.md` + ADR 0001 (stack .NET 10 / React / PostgreSQL / AWS a definir; multi-tenant; MVP clínico + agenda + auditoría; Fase 2 estudios/S3; recetas después). Correr `/revisar-paso` y esperar OK. Sin código.

### Qué generó la IA

- Propuesta de migración, ADR 0001, índice de ADRs, registro y commit.

### Qué revisó o debe revisar el desarrollador

- Matriz de roles (Recepción sin HC) y la estrategia greenfield vs strangler.
- OK para arrancar foundation.

### Tiempo estimado ahorrado

~1–1.5 h de redacción de propuesta y ADR.

---

## 2026-10-22 — ajuste roadmap AWS

### Pedido principal

Mover Cloud AWS a Fase 2 en propuesta y ADR 0001; nuevo orden 1–5. Luego arrancar Fase 1 Paso 1.

### Qué generó la IA

- Edits de números de fase en `migration-proposal.md` y ADR 0001.

### Qué revisó o debe revisar el desarrollador

- Que el orden coincida con la prioridad de demo pública.

### Tiempo estimado ahorrado

~10 min.

---

## 2026-10-22 — Fase 1 / Paso 1

### Pedido principal

Solución .NET 10 Clean Architecture (proyectos, CPM, editorconfig, tests xUnit + FluentAssertions, ADR 0002). Lecciones de Pymen desde el inicio. Commit, `/revisar-paso`, esperar OK.

### Qué generó la IA

- Scaffold de solución/proyectos, tipos base de dominio, `/health`, ADR 0002, registro.

### Qué revisó o debe revisar el desarrollador

- Layout de capas y el ADR 0002.

### Tiempo estimado ahorrado

~1–1.5 h de armado de solución.

---

## 2026-10-22 — Fase 1 / Paso 2

### Pedido principal

Dominio clínico + tests de evolución firmada no editable, sin solape de turnos, vigencia de diagnósticos y auditoría append-only.

### Qué generó la IA

- Entidades de dominio y suite de tests (32 de dominio).

### Qué revisó o debe revisar el desarrollador

- Transiciones de `ProgressNote` y `Appointment`, y la regla de un diagnóstico vigente.

### Tiempo estimado ahorrado

~2–3 h de modelado y tests de máquina de estados.
