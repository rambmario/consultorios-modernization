# Decisiones del dueño del sistema (Mario)

**Fecha:** 2026-10-22  
**Contexto:** Cierre de las preguntas abiertas del análisis legacy (Fase 0 / Paso 2).  
**Fuente:** confirmación explícita del dueño del sistema.

---

## Decisiones cerradas

Cerradas el **2026-10-22**.

### 1. Una sola clínica

Se usó en **una sola clínica**. El legado es monoclínica y monousuario. El producto nuevo, por decisión de producto, será **multi-tenant por consultorio/clínica** (ver propuesta de migración).

### 2. Un solo profesional

Lo usaba **un solo médico (cardiólogo)**. No había recepción ni otros usuarios.

### 3. Login

Hubo login en una **versión que no está en este árbol**. El producto nuevo tiene **login obligatorio**.

### 4. Agenda

No había agenda en el sistema. El producto nuevo **crea la agenda de turnos desde cero** y **entra en el MVP**.

### 5. Recetas

**Fase posterior**, fuera del MVP.

### 6. Obra social

Alcanza con **nombre de la obra social + número de afiliado**. Sin facturación ni liquidación.

### 7. Estudios y adjuntos

Pedidos de estudio, resultados y ecodoppler: **Fase 2**. Los adjuntos pasan a **almacenamiento de objetos (S3)** con control de acceso; **nunca rutas locales**.

### 8. Diagnósticos

**Historial de diagnósticos** por paciente (no un único diagnóstico actual).

### 9. Siglas de anamnesis

El dueño confirma que son **siglas médicas** (el sistema lo usaba un cardiólogo). **Interpretación asumida, marcada como tal:**

| Sigla | Interpretación asumida |
|-------|------------------------|
| MC | Motivo de consulta |
| AEA | Antecedentes de la enfermedad actual |
| APP | Antecedentes personales patológicos |
| AF | Antecedentes familiares |
| EF | Examen físico |
| AH | Antecedentes de hábitos (tabaquismo, alcohol, actividad física) |

Se migran como **secciones estructuradas** de la anamnesis, con **texto largo**.

### 10. Esquema de referencia

El de las clases `cls*` (**paciente con 21 columnas**), no el de `Consultas/CREAR_TABLAS.sql`.

### 11. Stored procedures / HC

No hay backup de esquema utilizable. La historia clínica se **reimplementa** a partir de este análisis.

### 12. Longitud de textos

Evoluciones y textos de anamnesis: **texto largo** (sin el tope de 200 caracteres).

### 13. Datos

**No se migran datos reales**: solo el comportamiento, con seed **100 % ficticio**.

### 14. Firma del profesional

**Profesional firmante obligatorio** en cada evolución desde el día uno.

### 15. Copias ajenas en la carpeta legacy

Las **copias de otros sistemas en la carpeta** se descartan por completo: no se analizan, no se migran y **no se nombran** en los documentos públicos.

### 16. Archivos de base y certificados

Los `.mdf`, `.ldf` y `.pfx` del legacy se **ignoran** y **nunca se copian**.

---

## Preguntas abiertas

Ninguna de las 16 originales. Quedan solo definiciones de diseño para fases posteriores (detalle de UX de agenda, matriz fina de permisos de Recepción sobre la ficha, etc.), a resolver cuando corresponda, sin reabrir lo de arriba.

---

## Impacto en documentos

- `legacy-analysis.md`: alineado a estas decisiones; menciones a copias ajenas reemplazadas por la fórmula acordada.
- `migration-proposal.md` y ADR 0001 (Paso 3): parten de este cierre.
