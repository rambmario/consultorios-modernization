import {
  Alert,
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography
} from "@mui/material";
import { FormEvent, useEffect, useState } from "react";
import { ApiError } from "../api/client";
import {
  useAppointmentsQuery,
  useCreateAppointment,
  usePatientsQuery,
  useProfessionalsQuery
} from "../api/hooks";
import type { CreateAppointmentRequest } from "../api/models";
import { appointmentStatusLabels, labelOf } from "../api/models";
import { formatDateTime } from "../i18n/dates";

export function AppointmentsPage() {
  const appointments = useAppointmentsQuery();
  const patients = usePatientsQuery("", 1, 50);
  const professionals = useProfessionalsQuery();
  const create = useCreateAppointment();
  const [form, setForm] = useState<CreateAppointmentRequest>({
    patientId: "",
    professionalId: "",
    startsAt: "",
    endsAt: ""
  });

  useEffect(() => {
    setForm((current) => ({
      ...current,
      patientId: current.patientId || patients.data?.items[0]?.id || "",
      professionalId: current.professionalId || professionals.data?.[0]?.id || ""
    }));
  }, [patients.data, professionals.data]);

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    try {
      await create.mutateAsync({
        ...form,
        startsAt: new Date(form.startsAt).toISOString(),
        endsAt: new Date(form.endsAt).toISOString()
      });
    } catch {
      // Error surfaces via create.error
    }
  }

  const loadError =
    appointments.error instanceof ApiError
      ? appointments.error.message
      : appointments.error || patients.error || professionals.error
        ? "No se pudo cargar la agenda."
        : null;
  const createError =
    create.error instanceof ApiError
      ? create.error.message
      : create.error
        ? "No se pudo agendar el turno."
        : null;

  return (
    <Stack spacing={3} component="section">
      <Typography variant="h1">Agenda</Typography>
      {loadError ? <Alert severity="error">{loadError}</Alert> : null}
      {appointments.isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: 4 }} role="status">
          <CircularProgress aria-label="Cargando agenda" />
        </Box>
      ) : (
        <Paper>
          <Table aria-label="Turnos">
            <TableHead>
              <TableRow>
                <TableCell>Inicio</TableCell>
                <TableCell>Fin</TableCell>
                <TableCell>Estado</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(appointments.data?.items ?? []).map((item) => (
                <TableRow key={item.id}>
                  <TableCell>{formatDateTime(item.startsAt)}</TableCell>
                  <TableCell>{formatDateTime(item.endsAt)}</TableCell>
                  <TableCell>{labelOf(appointmentStatusLabels, item.status)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      )}

      <Paper component="form" onSubmit={onCreate} sx={{ p: 2 }}>
        <Typography variant="h2" gutterBottom>
          Nuevo turno
        </Typography>
        {createError ? (
          <Alert severity="error" sx={{ mb: 2 }}>
            {createError}
          </Alert>
        ) : null}
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))",
            gap: 1.5,
            mb: 2
          }}
        >
          <TextField
            select
            label="Paciente"
            value={form.patientId}
            onChange={(event) => setForm({ ...form, patientId: event.target.value })}
          >
            {(patients.data?.items ?? []).map((patient) => (
              <MenuItem key={patient.id} value={patient.id}>
                {patient.lastName}, {patient.firstName}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            select
            label="Profesional"
            value={form.professionalId}
            onChange={(event) => setForm({ ...form, professionalId: event.target.value })}
          >
            {(professionals.data ?? []).map((professional) => (
              <MenuItem key={professional.id} value={professional.id}>
                {professional.email}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            label="Inicio"
            type="datetime-local"
            value={form.startsAt}
            onChange={(event) => setForm({ ...form, startsAt: event.target.value })}
            required
            slotProps={{ inputLabel: { shrink: true } }}
          />
          <TextField
            label="Fin"
            type="datetime-local"
            value={form.endsAt}
            onChange={(event) => setForm({ ...form, endsAt: event.target.value })}
            required
            slotProps={{ inputLabel: { shrink: true } }}
          />
        </Box>
        <Button type="submit" disabled={create.isPending}>
          {create.isPending ? "Agendando…" : "Agendar"}
        </Button>
      </Paper>
    </Stack>
  );
}
