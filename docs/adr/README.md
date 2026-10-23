# Architecture Decision Records (ADR)

Decisiones de arquitectura del proyecto, una por archivo.

## Formato

`NNNN-titulo.md` (numeración de 4 dígitos, título en kebab-case).

Secciones obligatorias:

1. **Contexto** — el problema y las restricciones.
2. **Decisión** — qué se eligió.
3. **Alternativas consideradas** — qué se descartó y por qué.
4. **Consecuencias** — efectos positivos, trade-offs y seguimiento.

Documentación en **español**.

## Índice

| ADR | Título | Estado |
|-----|--------|--------|
| [0001](0001-estrategia-de-modernizacion.md) | Estrategia de modernización (greenfield por slices) | Aceptado |
| [0002](0002-estructura-de-solucion.md) | Estructura de la solución (.NET 10 Clean Architecture) | Aceptado |
| [0003](0003-aislamiento-tenants-fail-closed.md) | Aislamiento de tenants fail-closed | Aceptado |
| [0004](0004-persistencia-ef-core-postgresql.md) | Persistencia EF Core + PostgreSQL y exclusion de turnos | Aceptado |
