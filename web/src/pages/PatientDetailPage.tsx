import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api, ApiError } from "../api/client";
import type { PatientResponse } from "../api/models";
import { useAuth } from "../auth/AuthContext";
import { canAccessClinicalRecord } from "../auth/roles";

export function PatientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { session } = useAuth();
  const [patient, setPatient] = useState<PatientResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const showClinical = canAccessClinicalRecord(session?.roles ?? []);

  useEffect(() => {
    if (!id) {
      return;
    }
    api<PatientResponse>(`/api/patients/${id}`)
      .then(setPatient)
      .catch((err) => setError(err instanceof ApiError ? err.message : "No se encontró la ficha."));
  }, [id]);

  if (error) {
    return <p className="error">{error}</p>;
  }
  if (!patient) {
    return <p>Cargando ficha…</p>;
  }

  return (
    <section>
      <p>
        <Link to="/patients">← Pacientes</Link>
      </p>
      <h1>
        {patient.lastName}, {patient.firstName}
      </h1>
      <dl className="facts">
        <div>
          <dt>Documento</dt>
          <dd>{patient.nationalId}</dd>
        </div>
        <div>
          <dt>Teléfono</dt>
          <dd>{patient.phone ?? "—"}</dd>
        </div>
        <div>
          <dt>Obra social</dt>
          <dd>
            {patient.coverage
              ? `${patient.coverage.healthPlanName} · ${patient.coverage.affiliationNumber}`
              : "—"}
          </dd>
        </div>
        <div>
          <dt>Dirección</dt>
          <dd>
            {[patient.address.street, patient.address.number, patient.address.city].filter(Boolean).join(" ") || "—"}
          </dd>
        </div>
      </dl>
      {showClinical ? (
        <Link className="button-link" to={`/patients/${patient.id}/clinical-record`}>
          Abrir historia clínica
        </Link>
      ) : (
        <p className="muted">La historia clínica solo la abre un profesional (queda auditada).</p>
      )}
    </section>
  );
}
