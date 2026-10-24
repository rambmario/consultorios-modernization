import {
  Alert,
  Box,
  CircularProgress,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography
} from "@mui/material";
import { ApiError } from "../api/client";
import { useAuditsQuery } from "../api/hooks";
import { auditActionLabels, labelOf } from "../api/models";
import { formatDateTime } from "../i18n/dates";

export function AuditPage() {
  const audits = useAuditsQuery();
  const error =
    audits.error instanceof ApiError
      ? audits.error.message
      : audits.error
        ? "No se pudo cargar la auditoría."
        : null;

  return (
    <Stack spacing={2} component="section">
      <Typography variant="h1">Auditoría de historia clínica</Typography>
      <Typography color="text.secondary">
        Quién vio o modificó qué, y cuándo. Datos de demostración ficticios.
      </Typography>
      {error ? <Alert severity="error">{error}</Alert> : null}
      {audits.isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: 4 }} role="status">
          <CircularProgress aria-label="Cargando auditoría" />
        </Box>
      ) : (
        <Paper>
          <Table aria-label="Auditoría de historia clínica">
            <TableHead>
              <TableRow>
                <TableCell>Cuándo</TableCell>
                <TableCell>Acción</TableCell>
                <TableCell>Recurso</TableCell>
                <TableCell>Actor</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(audits.data ?? []).map((row) => (
                <TableRow key={row.id}>
                  <TableCell>{formatDateTime(row.occurredAt)}</TableCell>
                  <TableCell>{labelOf(auditActionLabels, row.action)}</TableCell>
                  <TableCell>
                    {row.resourceType} · {row.resourceId.slice(0, 8)}
                  </TableCell>
                  <TableCell>{row.actorUserId.slice(0, 8)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      )}
    </Stack>
  );
}
