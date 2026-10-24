import { Alert, Box, Paper, Stack, Tab, Tabs, Typography } from "@mui/material";
import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import type { ClinicalRecordResponse, PatientResponse } from "../api/models";
import { canAccessClinicalRecord } from "../auth/roles";
import { ClinicalRecordPanel } from "../clinical/ClinicalRecordPanel";
import { formatDateTime } from "../i18n/dates";

export interface PatientFileProps {
  patient: PatientResponse;
  roles: readonly string[];
  clinicalRecord?: ClinicalRecordResponse | null;
  isLoadingRecord?: boolean;
  recordError?: string | null;
  isSigning?: boolean;
  onSignNote?: (body: string) => Promise<void> | void;
  onOpenClinicalTab?: () => void;
}

export function PatientFile({
  patient,
  roles,
  clinicalRecord = null,
  isLoadingRecord = false,
  recordError = null,
  isSigning = false,
  onSignNote,
  onOpenClinicalTab
}: PatientFileProps) {
  const showClinical = canAccessClinicalRecord(roles);
  const [tab, setTab] = useState(0);

  function changeTab(_: unknown, next: number) {
    setTab(next);
    if (next === 1) {
      onOpenClinicalTab?.();
    }
  }

  return (
    <Box component="section">
      <Typography variant="body2" sx={{ mb: 1 }}>
        <RouterLink to="/patients">← Pacientes</RouterLink>
      </Typography>
      <Typography variant="h1" gutterBottom>
        {patient.lastName}, {patient.firstName}
      </Typography>
      <Tabs value={tab} onChange={changeTab} aria-label="Secciones de la ficha" sx={{ mb: 2 }}>
        <Tab label="Ficha" id="patient-tab-file" aria-controls="patient-panel-file" />
        {showClinical ? (
          <Tab
            label="Historia clínica"
            id="patient-tab-clinical"
            aria-controls="patient-panel-clinical"
          />
        ) : null}
      </Tabs>

      {tab === 0 ? (
        <Paper id="patient-panel-file" role="tabpanel" aria-labelledby="patient-tab-file" sx={{ p: 2 }}>
          <Stack spacing={1.5}>
            <Fact label="Documento" value={patient.nationalId} />
            <Fact label="Teléfono" value={patient.phone ?? "—"} />
            <Fact
              label="Obra social"
              value={
                patient.coverage
                  ? `${patient.coverage.healthPlanName} · ${patient.coverage.affiliationNumber}`
                  : "—"
              }
            />
            <Fact
              label="Dirección"
              value={
                [patient.address.street, patient.address.number, patient.address.city]
                  .filter(Boolean)
                  .join(" ") || "—"
              }
            />
            <Fact label="Primera consulta" value={formatDateTime(patient.firstConsultationAt)} />
          </Stack>
        </Paper>
      ) : null}

      {showClinical && tab === 1 ? (
        <Box id="patient-panel-clinical" role="tabpanel" aria-labelledby="patient-tab-clinical">
          <ClinicalRecordPanel
            record={clinicalRecord}
            isLoading={isLoadingRecord}
            error={recordError}
            isSigning={isSigning}
            onSign={onSignNote}
          />
        </Box>
      ) : null}

      {!showClinical ? (
        <Alert severity="info" sx={{ mt: 2 }}>
          La historia clínica solo la abre un profesional (queda auditada).
        </Alert>
      ) : null}
    </Box>
  );
}

function Fact({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography>{value}</Typography>
    </Box>
  );
}
