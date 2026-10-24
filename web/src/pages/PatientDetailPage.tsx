import { Alert, CircularProgress, Box } from "@mui/material";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { ApiError } from "../api/client";
import { useClinicalRecordQuery, usePatientQuery, useSignProgressNote } from "../api/hooks";
import { useAuth } from "../auth/AuthContext";
import { canAccessClinicalRecord } from "../auth/roles";
import { PatientFile } from "../patients/PatientFile";

export function PatientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { session } = useAuth();
  const roles = session?.roles ?? [];
  const showClinical = canAccessClinicalRecord(roles);
  const [clinicalOpen, setClinicalOpen] = useState(false);
  const patient = usePatientQuery(id);
  const record = useClinicalRecordQuery(id, showClinical && clinicalOpen);
  const sign = useSignProgressNote(id);

  if (patient.isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: 4 }} role="status">
        <CircularProgress aria-label="Cargando ficha" />
      </Box>
    );
  }

  if (patient.error || !patient.data) {
    const message =
      patient.error instanceof ApiError ? patient.error.message : "No se encontró la ficha.";
    return <Alert severity="error">{message}</Alert>;
  }

  const recordError =
    record.error instanceof ApiError
      ? record.error.message
      : record.error
        ? "No se pudo abrir la HC."
        : sign.error instanceof ApiError
          ? sign.error.message
          : sign.error
            ? "No se pudo firmar la evolución."
            : null;

  return (
    <PatientFile
      patient={patient.data}
      roles={roles}
      clinicalRecord={record.data ?? null}
      isLoadingRecord={record.isLoading}
      recordError={recordError}
      isSigning={sign.isPending}
      onOpenClinicalTab={() => setClinicalOpen(true)}
      onSignNote={async (body) => {
        await sign.mutateAsync(body);
      }}
    />
  );
}
