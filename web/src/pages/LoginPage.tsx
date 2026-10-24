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
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  if (session) {
    return <Navigate to="/patients" replace />;
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    if (!email.includes("@") || password.length < 8) {
      setError("Ingresá un email válido y una contraseña de al menos 8 caracteres.");
      return;
    }
    setBusy(true);
    try {
      await login(email, password);
    } catch (err) {
      setError(err instanceof ApiError && err.status === 401 ? "Credenciales inválidas." : "No se pudo iniciar sesión.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="login-screen">
      <form className="card login-card" onSubmit={onSubmit}>
        <p className="eyebrow">Consultorios</p>
        <h1>Ingreso al consultorio</h1>
        <p className="muted">Usuarios demo ficticios. Las contraseñas viven solo en tu `.env`.</p>
        <label>
          Email
          <input value={email} onChange={(e) => setEmail(e.target.value)} autoComplete="username" />
        </label>
        <label>
          Contraseña
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
          />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <button type="submit" disabled={busy}>
          {busy ? "Ingresando…" : "Ingresar"}
        </button>
        <ul className="demo-list">
          {demoUsers.map((user) => (
            <li key={user}>{user}</li>
          ))}
        </ul>
      </form>
    </div>
  );
}
