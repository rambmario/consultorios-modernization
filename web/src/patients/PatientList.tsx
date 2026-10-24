import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography
} from "@mui/material";
import { FormEvent } from "react";
import { Link as RouterLink } from "react-router-dom";
import type { PatientResponse } from "../api/models";

export interface PatientListProps {
  items: PatientResponse[];
  search: string;
  onSearchChange: (value: string) => void;
  onSearch: () => void;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onPageSizeChange?: (pageSize: number) => void;
  isLoading?: boolean;
  error?: string | null;
}

export function PatientList({
  items,
  search,
  onSearchChange,
  onSearch,
  page,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
  isLoading = false,
  error = null
}: PatientListProps) {
  function submitSearch(event: FormEvent) {
    event.preventDefault();
    onSearch();
  }

  return (
    <Box component="section">
      <Typography variant="h1" gutterBottom>
        Pacientes
      </Typography>
      <Box
        component="form"
        role="search"
        onSubmit={submitSearch}
        sx={{ display: "flex", gap: 1.5, mb: 2, alignItems: "center" }}
      >
        <TextField
          label="Buscar por apellido, nombre o documento"
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          fullWidth
        />
        <Button type="submit">Buscar</Button>
      </Box>

      {error ? (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      ) : null}

      {isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: 4 }} role="status">
          <CircularProgress aria-label="Cargando pacientes" />
        </Box>
      ) : (
        <TableContainer component={Paper}>
          <Table aria-label="Lista de pacientes">
            <TableHead>
              <TableRow>
                <TableCell>Nombre</TableCell>
                <TableCell>Documento</TableCell>
                <TableCell>Obra social</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {items.map((patient) => (
                <TableRow key={patient.id}>
                  <TableCell>
                    <RouterLink to={`/patients/${patient.id}`}>
                      {patient.lastName}, {patient.firstName}
                    </RouterLink>
                  </TableCell>
                  <TableCell>{patient.nationalId}</TableCell>
                  <TableCell>{patient.coverage?.healthPlanName ?? "—"}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={totalCount}
            page={page - 1}
            onPageChange={(_, nextPage) => onPageChange(nextPage + 1)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={
              onPageSizeChange
                ? (event) => onPageSizeChange(Number.parseInt(event.target.value, 10))
                : undefined
            }
            rowsPerPageOptions={onPageSizeChange ? [5, 10, 25] : [pageSize]}
            labelRowsPerPage="Filas por página"
            labelDisplayedRows={({ from, to, count }) => `${from}–${to} de ${count}`}
            getItemAriaLabel={(type) =>
              type === "next" ? "Ir a la página siguiente" : "Ir a la página anterior"
            }
          />
        </TableContainer>
      )}
    </Box>
  );
}
