import { FormEvent, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api, ApiError } from "../api/client";
import type { CreatePatientRequest, PagedResult, PatientResponse } from "../api/models";

export function PatientsPage() {
  const [patients, setPatients] = useState<PatientResponse[]>([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<CreatePatientRequest>({
    lastName: "",
    firstName: "",
    nationalId: ""
  });

  async function load(term = search) {
    const query = term ? `?search=${encodeURIComponent(term)}` : "";
    const page = await api<PagedResult<PatientResponse>>(`/api/patients${query}`);
    setPatients(page.items);
  }

  useEffect(() => {
    load().catch((err) => setError(err instanceof ApiError ? err.message : "No se pudieron cargar pacientes."));
  }, []);

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    setError(null);
    try {
      await api<PatientResponse>("/api/patients", {
        method: "POST",
        body: JSON.stringify(form)
      });
      setForm({ lastName: "", firstName: "", nationalId: "" });
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo crear el paciente.");
    }
  }

  return (
    <section>
      <h1>Pacientes</h1>
      <form
        className="toolbar"
        onSubmit={(event) => {
          event.preventDefault();
          load().catch((err) => setError(err instanceof Error ? err.message : "Error"));
        }}
      >
        <input
          placeholder="Buscar por apellido, nombre o documento"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <button type="submit">Buscar</button>
      </form>

      {error ? <p className="error">{error}</p> : null}

      <table>
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Documento</th>
            <th>Obra social</th>
          </tr>
        </thead>
        <tbody>
          {patients.map((patient) => (
            <tr key={patient.id}>
              <td>
                <Link to={`/patients/${patient.id}`}>
                  {patient.lastName}, {patient.firstName}
                </Link>
              </td>
              <td>{patient.nationalId}</td>
              <td>{patient.coverage?.healthPlanName ?? "—"}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <form className="card" onSubmit={onCreate}>
        <h2>Alta ficticia</h2>
        <div className="grid">
          <label>
            Apellido
            <input value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} required />
          </label>
          <label>
            Nombre
            <input value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} required />
          </label>
          <label>
            Documento
            <input value={form.nationalId} onChange={(e) => setForm({ ...form, nationalId: e.target.value })} required />
          </label>
        </div>
        <button type="submit">Guardar paciente</button>
      </form>
    </section>
  );
}
