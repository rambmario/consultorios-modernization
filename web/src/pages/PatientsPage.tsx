import { Alert, Box, Button, Paper, Stack, TextField, Typography } from "@mui/material";
import { FormEvent, useState } from "react";
import { ApiError } from "../api/client";
import { useCreatePatient, usePatientsQuery } from "../api/hooks";
import type { CreatePatientRequest } from "../api/models";
import { PatientList } from "../patients/PatientList";

export function PatientsPage() {
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const patients = usePatientsQuery(search, page, pageSize);
  const create = useCreatePatient();
  const [form, setForm] = useState<CreatePatientRequest>({
    lastName: "",
    firstName: "",
    nationalId: ""
  });

  function applySearch() {
    setPage(1);
    setSearch(searchInput);
  }

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    try {
      await create.mutateAsync(form);
      setForm({ lastName: "", firstName: "", nationalId: "" });
    } catch {
      // Error surfaces via create.error
    }
  }

  const queryError =
    patients.error instanceof ApiError
      ? patients.error.message
      : patients.error
        ? "No se pudieron cargar pacientes."
        : null;
  const createError =
    create.error instanceof ApiError
      ? create.error.message
      : create.error
        ? "No se pudo crear el paciente."
        : null;

  return (
    <Stack spacing={3}>
      <PatientList
        items={patients.data?.items ?? []}
        search={searchInput}
        onSearchChange={setSearchInput}
        onSearch={applySearch}
        page={page}
        pageSize={pageSize}
        totalCount={patients.data?.totalCount ?? 0}
        onPageChange={setPage}
        onPageSizeChange={(next) => {
          setPageSize(next);
          setPage(1);
        }}
        isLoading={patients.isLoading}
        error={queryError}
      />

      <Paper component="form" onSubmit={onCreate} sx={{ p: 2 }}>
        <Typography variant="h2" gutterBottom>
          Alta ficticia
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
            label="Apellido"
            value={form.lastName}
            onChange={(event) => setForm({ ...form, lastName: event.target.value })}
            required
          />
          <TextField
            label="Nombre"
            value={form.firstName}
            onChange={(event) => setForm({ ...form, firstName: event.target.value })}
            required
          />
          <TextField
            label="Documento"
            value={form.nationalId}
            onChange={(event) => setForm({ ...form, nationalId: event.target.value })}
            required
          />
        </Box>
        <Button type="submit" disabled={create.isPending}>
          {create.isPending ? "Guardando…" : "Guardar paciente"}
        </Button>
      </Paper>
    </Stack>
  );
}
