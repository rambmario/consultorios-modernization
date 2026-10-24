import { Alert, Box, Button, Paper, Stack, TextField, Typography } from "@mui/material";
import { useMutation } from "@tanstack/react-query";
import { FormEvent, useState } from "react";
import { Navigate } from "react-router-dom";
import { ApiError } from "../api/client";
import { useAuth } from "../auth/AuthContext";

const demoUsers = [
  "profesional.clinicaa@demo.local",
  "recepcion.clinicaa@demo.local",
  "admin.clinicaa@demo.local"
];

export function LoginPage() {
  const { session, login } = useAuth();
  const [email, setEmail] = useState(demoUsers[0]);
  const [password, setPassword] = useState("");
  const [validationError, setValidationError] = useState<string | null>(null);
  const mutation = useMutation({
    mutationFn: ({ nextEmail, nextPassword }: { nextEmail: string; nextPassword: string }) =>
      login(nextEmail, nextPassword)
  });

  if (session) {
    return <Navigate to="/patients" replace />;
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setValidationError(null);
    if (!email.includes("@") || password.length < 8) {
      setValidationError("Ingresá un email válido y una contraseña de al menos 8 caracteres.");
      return;
    }
    try {
      await mutation.mutateAsync({ nextEmail: email, nextPassword: password });
    } catch {
      // Error surfaces via mutation.error
    }
  }

  const submitError =
    mutation.error instanceof ApiError && mutation.error.status === 401
      ? "Credenciales inválidas."
      : mutation.error
        ? "No se pudo iniciar sesión."
        : null;

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "grid",
        placeItems: "center",
        px: 2
      }}
    >
      <Paper component="form" onSubmit={onSubmit} sx={{ width: "min(420px, 100%)", p: 3 }}>
        <Stack spacing={2}>
          <Typography variant="overline" color="text.secondary">
            Consultorios
          </Typography>
          <Typography variant="h1">Ingreso al consultorio</Typography>
          <Typography color="text.secondary">
            Usuarios demo ficticios. Las contraseñas viven solo en tu `.env`.
          </Typography>
          <TextField
            label="Email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="username"
          />
          <TextField
            label="Contraseña"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
          />
          {validationError || submitError ? (
            <Alert severity="error">{validationError ?? submitError}</Alert>
          ) : null}
          <Button type="submit" disabled={mutation.isPending}>
            {mutation.isPending ? "Ingresando…" : "Ingresar"}
          </Button>
          <Box component="ul" sx={{ color: "text.secondary", pl: 2.5, m: 0 }}>
            {demoUsers.map((user) => (
              <li key={user}>{user}</li>
            ))}
          </Box>
        </Stack>
      </Paper>
    </Box>
  );
}
