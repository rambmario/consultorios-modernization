# AGENTS.md — Consultorios modernization

Guidance for AI coding agents (Cursor, Claude Code, Codex) working in this repo.

## Project

Modernization of Consultorios, a legacy clinical-records system I built at Ingesoft, into .NET 10 Web API (Clean Architecture) + React/TypeScript (Vite) + PostgreSQL, targeting AWS later. Legacy analysis lives in `docs/legacy/`. This is health software: privacy rules override everything else.

## Privacy (mandatory)

Never copy into this repo:

- Real patient, professional, or clinic data: names, national IDs (DNI/CUIT/CUIL), addresses, phones, emails, dates of birth, diagnoses, treatments, real progress notes, prescriptions, real appointments, attachments, images, studies.
- Database backups or files (`.bak`, `.mdf`, `.ldf`, `.ndf`).
- Connection strings, passwords, keys, tokens, or credentials.

If real data appears while reading the legacy tree (scripts, seeds, reports, config):

- Do not quote it. Describe structure only (e.g. "Paciente table with 25 columns").
- Replace clinic or doctor names with `ClinicaA`, `ProfesionalA`.
- Example data must be obviously fictitious (e.g. `Paciente Demo 1`).

The legacy folder `D:\VBNetAnterior\Consultorios` is **read-only**. Never modify, move, copy, or delete anything there.

## Architecture rules

- Clean Architecture: Domain has no dependencies; Application depends only on Domain; Infrastructure and Api depend inward. Never reference Infrastructure from Domain or Application.
- Business rules live in Domain (entities, value objects, state transitions), not in controllers or EF configurations.
- Every tenant-owned entity implements a tenant boundary; never bypass the global tenant query filter.
- Clinical-record access must be auditable (who viewed or changed what, and when).
- Names in English in code; business terms explained in Spanish in docs.
- Only migrate the modules and flows defined in `docs/legacy/migration-proposal.md`; anything else in the legacy code is out of scope.

## Quality

- Every domain rule and state transition has a unit test (xUnit).
- Tenant isolation and clinical-record audit must stay covered by tests.
- No secrets, connection strings, or real clinical data in the repo; seeds use fictitious data only.
- Run `dotnet build` / `dotnet test` and the frontend test suite before committing; CI must stay green.

## Workflow

- One task = one commit, Conventional Commits.
- Log every session as defined in `.cursor/rules/registro.mdc` (worklog, ai-usage, ADR, CHANGELOG).
- Docs in Spanish; code, commits, and README in English.
- If legacy behavior is unclear, add a question to `docs/legacy/owner-decisions.md` instead of guessing.
- Do not write application code until the owner approves the analysis and the migration proposal.
