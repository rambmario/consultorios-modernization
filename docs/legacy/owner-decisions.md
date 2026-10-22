# Decisiones del dueño del sistema (Mario)

**Fecha:** 2026-10-22  
**Contexto:** Preguntas abiertas del análisis legacy (Fase 0 / Paso 2).  
**Regla:** no adivinar. Cada ítem se cierra solo con confirmación explícita del dueño.

---

## Decisiones cerradas

(Ninguna todavía. Se completan cuando Mario responda.)

---

## Preguntas abiertas

### Uso real

1. ¿Consultorios se usó en **una sola clínica** o hubo varias instalaciones (cada una con su SQL local)?
2. ¿Lo usaba **un solo profesional**, varios, y/o recepción? ¿Alguien más (administración)?
3. ¿Hubo **login** en alguna versión que no está en este árbol, o el EXE siempre se abrió sin usuario?

### Agenda, recetas, facturación

4. En el código **no hay agenda/turnos**. ¿Se manejaban en papel, Excel u otro sistema? ¿El producto nuevo debe **crear la agenda desde cero**?
5. En el código **no hay recetas**. ¿Se hacían a mano / Word / otro medio? ¿Van en el producto nuevo y en qué fase?
6. La **obra social** en legacy es un dato de ficha (nombre + nro. de afiliado), sin liquidación. ¿En el producto nuevo alcanza con eso, o se espera facturación / coseguro / presentación a OS? ¿En qué fase?

### Alcance clínico del legado vs MVP

7. ¿Los **pedidos de estudio**, resultados y el adjunto **ecodoppler** (`.doc` por path) entran en el MVP, en una fase posterior, o se descartan?
8. El catálogo de **diagnósticos** y el diagnóstico único en la ficha: ¿se mantiene “un diagnóstico actual”, o se quiere historial de diagnósticos?
9. Las siglas de anamnesis en la ficha (`MC`, `AEA`, `APP`, `AF`, `EF`, `AH`) aparecen en la UI solo como abreviaturas. ¿Cuál es el significado clínico de cada una? ¿Se migran como campos fijos o como notas libres de HC?

### Datos y esquema

10. Hay **drift** entre `Consultas/CREAR_TABLAS.sql` (paciente con 17 columnas; ecodoppler sin `archivo`) y las `cls*` (paciente con 21; ecodoppler con `archivo`). ¿Cuál es el esquema de la base que realmente se usó?
11. Los cuerpos de la mayoría de los **stored procedures** no están en el árbol (salvo el borrador de `GENERAR HC`). ¿Hay un backup de esquema (sin datos de pacientes) que se pueda usar después, o se reimplementa la HC a partir de este análisis?
12. `EVOLUCION.evolucion` está limitado a **200 caracteres**. ¿Eso fue un problema en el uso real? ¿En el nuevo sistema la evolución debe ser texto largo / con formato?

### Legal y retención

13. ¿Hay obligación concreta de **conservar** las HC del legado (plazos Ley 26.529) y, si sí, en qué soporte? ¿Se planea migrar datos (anonimizados / con consentimiento) o solo el comportamiento?
14. ¿Se necesita **firma / identificación del profesional** en cada evolución desde el día uno (la ley lo pide; el legado no lo tiene)?

### Ruido del árbol

15. ¿`Gestion_Agro` y los forms de sueldos/subalmacenes compilados en `Consultorios.vbproj` se **descartan por completo** (no se analizan más, no se migran)?
16. Los `.mdf` / `.ldf` y el `.pfx` que están en el folder legacy: ¿son copias de desarrollo y se ignoran? (Este repo **nunca** los va a copiar.)

---

## Impacto en documentos

- `legacy-analysis.md`: inventario y huecos del Paso 2.
- La propuesta de migración (Paso 3) no debe cerrar estas preguntas por su cuenta.
