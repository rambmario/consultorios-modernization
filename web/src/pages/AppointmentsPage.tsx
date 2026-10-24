import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import type {
  AppointmentResponse,
  CreateAppointmentRequest,
  PagedResult,
  PatientResponse,
  ProfessionalListItem
} from "../api/models";
import { appointmentStatusLabels, labelOf } from "../api/models";

export function AppointmentsPage() {
  const [appointments, setAppointments] = useState<AppointmentResponse[]>([]);
  const [patients, setPatients] = useState<PatientResponse[]>([]);
  const [professionals, setProfessionals] = useState<ProfessionalListItem[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<CreateAppointmentRequest>({
    patientId: "",
    professionalId: "",
    startsAt: "",
    endsAt: ""
  });

  async function load() {
    const [agenda, people, clinicians] = await Promise.all([
      api<PagedResult<AppointmentResponse>>("/api/appointments?pageSize=50"),
      api<PagedResult<PatientResponse>>("/api/patients?pageSize=50"),
      api<ProfessionalListItem[]>("/api/professionals")
    ]);
    setAppointments(agenda.items);
    setPatients(people.items);
    setProfessionals(clinicians);
    setForm((current) => ({
      ...current,
      patientId: current.patientId || people.items[0]?.id || "",
      professionalId: current.professionalId || clinicians[0]?.id || ""
    }));
  }

  useEffect(() => {
    load().catch((err) => setError(err instanceof ApiError ? err.message : "No se pudo cargar la agenda."));
  }, []);

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    setError(null);
    try {
      await api<AppointmentResponse>("/api/appointments", {
        method: "POST",
        body: JSON.stringify({
          ...form,
          startsAt: new Date(form.startsAt).toISOString(),
          endsAt: new Date(form.endsAt).toISOString()
        })
      });
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo agendar el turno.");
    }
  }

  return (
    <section>
      <h1>Agenda</h1>
      {error ? <p className="error">{error}</p> : null}
      <table>
        <thead>
          <tr>
            <th>Inicio</th>
            <th>Fin</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          {appointments.map((item) => (
            <tr key={item.id}>
              <td>{new Date(item.startsAt).toLocaleString()}</td>
              <td>{new Date(item.endsAt).toLocaleString()}</td>
              <td>{labelOf(appointmentStatusLabels, item.status)}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <form className="card" onSubmit={onCreate}>
        <h2>Nuevo turno</h2>
        <div className="grid">
          <label>
            Paciente
            <select value={form.patientId} onChange={(e) => setForm({ ...form, patientId: e.target.value })}>
              {patients.map((patient) => (
                <option key={patient.id} value={patient.id}>
                  {patient.lastName}, {patient.firstName}
                </option>
              ))}
            </select>
          </label>
          <label>
            Profesional
            <select
              value={form.professionalId}
              onChange={(e) => setForm({ ...form, professionalId: e.target.value })}
            >
              {professionals.map((professional) => (
                <option key={professional.id} value={professional.id}>
                  {professional.email}
                </option>
              ))}
            </select>
          </label>
          <label>
            Inicio
            <input
              type="datetime-local"
              value={form.startsAt}
              onChange={(e) => setForm({ ...form, startsAt: e.target.value })}
              required
            />
          </label>
          <label>
            Fin
            <input
              type="datetime-local"
              value={form.endsAt}
              onChange={(e) => setForm({ ...form, endsAt: e.target.value })}
              required
            />
          </label>
        </div>
        <button type="submit">Agendar</button>
      </form>
    </section>
  );
}
