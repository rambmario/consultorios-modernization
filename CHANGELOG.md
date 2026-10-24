# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Conventional Commits](https://www.conventionalcommits.org/).

## [Unreleased]

### Fixed

- Only the signing professional can cancel or correct a progress note; another professional gets `DomainException`.
- Health-plan types live in `HealthPlans/` so Windows gitignore `coverage/` does not drop them.

### Changed

- Frontend uses TanStack Query for every API call (cache, loading/error, invalidate on save) and MUI with a custom theme; dates render as `dd/MM/yyyy HH:mm` (es-AR); anamnesis sections show long Spanish names.
- ADR 0006 now includes TanStack Query and MUI as market-standard portfolio stack.
- Tenant and required-id checks live in `Domain/Common/Guard` (used by all tenant-owned entities).
- Roadmap: Cloud AWS is now Phase 2 (public demo) before studies/attachments, prescriptions, and reports.
- Owner decisions for Phase 0 recorded; open questions from the analysis closed (2026-10-22).
- Legacy analysis aligned to those decisions; copies of other systems in the legacy folder are out of scope and unnamed in public docs.
- Cursor review command lives in `.cursor/commands/revisar-paso.md`, with a clinical-data check and scope tied to `docs/legacy/migration-proposal.md`.
- README Author section aligned with the public profile (name, LinkedIn, GitHub).

### Added

- Recruiter-facing README (architecture, getting started, test table), MIT license, and fictitious Dev-stack screenshots (login, patients, clinical record).
- GitHub Actions CI (backend + frontend, Testcontainers, Coverlet) and Dependabot for NuGet, npm, and Actions.
- ADR 0007 (CI with GitHub Actions).
- React + TypeScript (Vite) client in `web/`: login, patients, agenda, clinical record, and HC audit, with role-gated navigation.
- Component tests for patient search/paging, file tabs by role, hidden HC for Recepción, and signing a progress note (20 Vitest tests).
- ADR 0006 (React/Vite frontend, TanStack Query, MUI).
- Web API with Identity + JWT (`tenant_id` claim), roles Admin / Profesional / Recepción, rate-limited login, and fail-closed 404 across tenants.
- Clinical-record audit on HC views and clinical writes; Recepción cannot read the HC.
- ADR 0005 (auth, roles, audit).
- Unit test pins `AppointmentStatus` integers used by the appointment exclusion constraint (`Scheduled = 0`, `Confirmed = 1`).
- EF Core + PostgreSQL persistence: fail-closed tenant filter, Restrict FKs, `InitialCreate` plus GiST exclusion constraint on overlapping appointments (`btree_gist`).
- docker-compose PostgreSQL 16, fictitious `ClinicaA`/`ClinicaB` seed, `/health` vs `/health/ready`.
- Testcontainers isolation, owned-address round-trip, and parallel overlap tests (0 skipped).
- ADR 0003 (fail-closed tenancy) and ADR 0004 (PostgreSQL + exclusion constraint).
- Clinical domain (tenant, patient, coverage, anamnesis, signed progress notes, diagnosis history, appointments, append-only audit) with unit tests.
- .NET 10 Clean Architecture solution (`Consultorios.sln`, `src/*`, `tests/*`) with CPM, `.editorconfig`, `global.json`.
- ADR 0002: solution structure.
- Migration proposal (`docs/legacy/migration-proposal.md`) and ADR 0001 (greenfield vertical slices).
- Legacy analysis (`docs/legacy/legacy-analysis.md`) and open owner questions (`docs/legacy/owner-decisions.md`).
- Repository scaffolding: README, CHANGELOG, `.gitignore` (Visual Studio, Node/Vite, `.env`, database files).
- Agent guidance (`AGENTS.md`) and Cursor registration rule (`.cursor/rules/registro.mdc`).
- Session logs: `docs/worklog.md`, `docs/ai-usage.md`, `docs/adr/`.
- Mandatory privacy rules for clinical-records software (no real patient, professional, or clinic data in the repo).
