import { Box, CircularProgress, Typography } from "@mui/material";
import { Link as RouterLink, useParams } from "react-router-dom";
import { ApiError } from "../api/client";
import { useClinicalRecordQuery, useSignProgressNote } from "../api/hooks";
import { ClinicalRecordPanel } from "../clinical/ClinicalRecordPanel";

export function ClinicalRecordPage() {
  const { id } = useParams<{ id: string }>();
  const record = useClinicalRecordQuery(id);
  const sign = useSignProgressNote(id);

  if (record.isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: 4 }} role="status">
        <CircularProgress aria-label="Cargando historia clínica" />
      </Box>
    );
  }

  const error =
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
    <Box>
      <Typography variant="body2" sx={{ mb: 1 }}>
        <RouterLink to={`/patients/${id}`}>← Ficha</RouterLink>
      </Typography>
      <ClinicalRecordPanel
        record={record.data ?? null}
        error={error}
        isSigning={sign.isPending}
        onSign={async (body) => {
          await sign.mutateAsync(body);
        }}
      />
    </Box>
  );
}
