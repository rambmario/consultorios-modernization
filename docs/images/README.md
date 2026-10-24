# Screenshots

PNG files used by the root README. Capture against a local Dev run (`docker compose up -d`, API on `:5088`, `cd web && npm run dev`).

Only fictitious seed data (`Paciente Demo 1`, `ClinicaA`, `*@demo.local`). Never include real patients, professionals, or passwords.

| File | What to capture |
|------|-----------------|
| `login.png` | Login screen (empty password). |
| `patients.png` | Patient list with search and pagination. |
| `clinical-record.png` | Clinical record tab: long anamnesis names and `dd/MM/yyyy HH:mm` dates. |

Regenerate from `web/` with `npm run screenshots` (Playwright; reads `DEMO_PASSWORD_PROFESIONAL` from the repo-root `.env`).
