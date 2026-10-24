import {
  Alert,
  Box,
  Button,
  Paper,
  Stack,
  TextField,
  Typography
} from "@mui/material";
import { FormEvent, useState } from "react";
import { labelOf, noteStatusLabels, type ClinicalRecordResponse } from "../api/models";
import { formatDateTime } from "../i18n/dates";
import { anamnesisSections } from "./anamnesisSections";

export interface ClinicalRecordPanelProps {
  record: ClinicalRecordResponse | null;
  hidden?: boolean;
  isLoading?: boolean;
  error?: string | null;
  isSigning?: boolean;
  onSign?: (body: string) => Promise<void> | void;
}

export function ClinicalRecordPanel({
  record,
  hidden = false,
  isLoading = false,
  error = null,
  isSigning = false,
  onSign
}: ClinicalRecordPanelProps) {
  const [body, setBody] = useState("");

  if (hidden) {
    return (
      <Alert severity="info">
        La historia clínica no está disponible para Recepción. Solo un profesional puede abrirla; la
        vista queda auditada.
      </Alert>
    );
  }

  if (error && !record) {
    return <Alert severity="error">{error}</Alert>;
  }

  if (isLoading || !record) {
    return <Typography>Cargando historia clínica…</Typography>;
  }

  async function submitSign(event: FormEvent) {
    event.preventDefault();
    if (!onSign) {
      return;
    }
    await onSign(body);
    setBody("");
  }

  return (
    <Stack spacing={2} component="section">
      <Typography variant="h2">Historia clínica</Typography>
      <Typography color="text.secondary">
        HC · {record.patient.lastName}, {record.patient.firstName}. Esta vista queda registrada en la
        auditoría.
      </Typography>
      {error ? <Alert severity="error">{error}</Alert> : null}

      <Paper sx={{ p: 2 }}>
        <Typography variant="h2" gutterBottom>
          Anamnesis
        </Typography>
        {anamnesisSections.map((section) => (
          <Typography key={section.key} sx={{ mb: 1 }}>
            <strong>{section.label}.</strong> {record.anamnesis[section.key] || "—"}
          </Typography>
        ))}
      </Paper>

      <Paper sx={{ p: 2 }}>
        <Typography variant="h2" gutterBottom>
          Diagnósticos
        </Typography>
        <Box component="ul" sx={{ m: 0, pl: 2.5 }}>
          {record.diagnoses.map((item) => (
            <li key={item.id}>
              {item.diagnosisName} {item.isCurrent ? "(vigente)" : ""}
            </li>
          ))}
        </Box>
      </Paper>

      <Paper sx={{ p: 2 }}>
        <Typography variant="h2" gutterBottom>
          Evoluciones
        </Typography>
        {record.progressNotes.map((note) => (
          <Box
            key={note.id}
            component="blockquote"
            sx={{ m: 0, mb: 2, pl: 1.5, borderLeft: 3, borderColor: "primary.main" }}
          >
            <Typography>{note.body}</Typography>
            <Typography variant="body2" color="text.secondary">
              {formatDateTime(note.signedAt)} · {labelOf(noteStatusLabels, note.status)}
            </Typography>
          </Box>
        ))}
        {onSign ? (
          <Box component="form" onSubmit={submitSign}>
            <TextField
              label="Nueva evolución (queda firmada)"
              value={body}
              onChange={(event) => setBody(event.target.value)}
              required
              multiline
              minRows={4}
              fullWidth
              sx={{ mb: 1.5 }}
            />
            <Button type="submit" disabled={isSigning}>
              {isSigning ? "Firmando…" : "Firmar"}
            </Button>
          </Box>
        ) : null}
      </Paper>
    </Stack>
  );
}
