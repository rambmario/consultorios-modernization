# Consultorios Modernization

Modernization of **Consultorios**, a legacy clinical-records system I designed and maintained at Ingesoft, into a multi-tenant web platform.

This repository is **greenfield documentation and code only**. The original desktop system is treated as read-only analysis input and is never copied here.

---

## Privacy

This is health software (clinical records). The repo must never contain:

- Real patient, professional, or clinic data (names, national IDs, addresses, phones, diagnoses, treatments, attachments, images).
- Database backups (`.bak`, `.mdf`, `.ldf`) or connection strings / credentials.

Seeds and screenshots use obviously fictitious data only (e.g. `Paciente Demo 1`, `ClinicaA`, `ProfesionalA`).

---

## Target stack (planned)

| Layer | Choice |
| --- | --- |
| API | .NET 10 Web API, Clean Architecture |
| UI | React + TypeScript (Vite) |
| Persistence | PostgreSQL + EF Core |
| Cloud | AWS (ECS Fargate or App Runner, RDS PostgreSQL, S3, SQS — to be decided later) |

Cross-cutting design goals: **multi-tenant by clinic**, **roles** (Admin, Profesional, Recepción), and **audit of clinical-record access**.

Proposed MVP: appointment agenda + patient file + clinical history with progress notes + audit trail.

---

## Repository layout

```
.
├── AGENTS.md                 # Rules for AI coding agents
├── CHANGELOG.md
├── docs/
│   ├── adr/                  # Architecture Decision Records
│   ├── ai-usage.md           # AI assistance log (portfolio transparency)
│   ├── worklog.md            # Session log
│   └── legacy/               # Legacy analysis and owner questions
└── .cursor/rules/            # Persistent project rules
```

Application source (`src/`, `web/`, tests) will be added after the owner approves the Phase 0 analysis.

---

## Status

**Phase 0 — discovery (complete).** Legacy analysis, owner decisions, [migration proposal](docs/legacy/migration-proposal.md), and [ADR 0001](docs/adr/0001-estrategia-de-modernizacion.md) are in `docs/`. No application code yet. Next: Phase 1 foundation after owner OK.

---

## Documentation language

- **English:** code, commit messages, this README.
- **Spanish:** analysis, ADRs, worklog, and other `docs/` files.

---

## Author

**Mario Ramb** — Senior .NET Full Stack Developer

- LinkedIn: [linkedin.com/in/marioramb](https://www.linkedin.com/in/marioramb)
- GitHub: [github.com/rambmario](https://github.com/rambmario)

Consultorios was originally built at Ingesoft.
