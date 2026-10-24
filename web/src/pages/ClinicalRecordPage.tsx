import { FormEvent, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api, ApiError } from "../api/client";
import type { ClinicalRecordResponse, ProgressNoteResponse } from "../api/models";
import { labelOf, noteStatusLabels } from "../api/models";

const anamnesisFields = [
  ["chiefComplaint", "Motivo de consulta"],
  ["currentIllnessHistory", "Antecedentes de la enfermedad actual"],
  ["personalPathologicalHistory", "Antecedentes personales patológicos"],
  ["familyHistory", "Antecedentes familiares"],
  ["physicalExamination", "Examen físico"],
  ["habitHistory", "Antecedentes de hábitos"]
] as const;

export function ClinicalRecordPage() {
  const { id } = useParams<{ id: string }>();
  const [record, setRecord] = useState<ClinicalRecordResponse | null>(null);
  const [body, setBody] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    if (!id) {
      return;
    }
    setRecord(await api<ClinicalRecordResponse>(`/api/patients/${id}/clinical-record`));
  }

  useEffect(() => {
    load().catch((err) => setError(err instanceof ApiError ? err.message : "No se pudo abrir la HC."));
  }, [id]);

  async function onSign(event: FormEvent) {
    event.preventDefault();
    if (!id) {
      return;
    }
    setError(null);
    try {
      await api<ProgressNoteResponse>(`/api/patients/${id}/progress-notes`, {
        method: "POST",
        body: JSON.stringify({ body })
      });
      setBody("");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo firmar la evolución.");
    }
  }

  if (error && !record) {
    return <p className="error">{error}</p>;
  }
  if (!record) {
    return <p>Cargando historia clínica…</p>;
  }

  return (
    <section>
      <p>
        <Link to={`/patients/${record.patient.id}`}>← Ficha</Link>
      </p>
      <h1>
        HC · {record.patient.lastName}, {record.patient.firstName}
      </h1>
      <p className="muted">Esta vista queda registrada en la auditoría.</p>
      {error ? <p className="error">{error}</p> : null}

      <article className="card">
        <h2>Anamnesis</h2>
        {anamnesisFields.map(([key, label]) => (
          <p key={key}>
            <strong>{label}.</strong> {record.anamnesis[key] || "—"}
          </p>
        ))}
      </article>

      <article className="card">
        <h2>Diagnósticos</h2>
        <ul>
          {record.diagnoses.map((item) => (
            <li key={item.id}>
              {item.diagnosisName} {item.isCurrent ? "(vigente)" : ""}
            </li>
          ))}
        </ul>
      </article>

      <article className="card">
        <h2>Evoluciones</h2>
        {record.progressNotes.map((note) => (
          <blockquote key={note.id}>
            <p>{note.body}</p>
            <footer>
              {new Date(note.signedAt).toLocaleString()} · {labelOf(noteStatusLabels, note.status)}
            </footer>
          </blockquote>
        ))}
        <form onSubmit={onSign}>
          <label>
            Nueva evolución (queda firmada)
            <textarea value={body} onChange={(e) => setBody(e.target.value)} required rows={4} />
          </label>
          <button type="submit">Firmar</button>
        </form>
      </article>
    </section>
  );
}
