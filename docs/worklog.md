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
