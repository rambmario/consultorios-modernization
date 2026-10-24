import { useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import type { ClinicalAuditResponse } from "../api/models";
import { auditActionLabels, labelOf } from "../api/models";

export function AuditPage() {
  const [rows, setRows] = useState<ClinicalAuditResponse[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api<ClinicalAuditResponse[]>("/api/clinical-audits")
      .then(setRows)
      .catch((err) => setError(err instanceof ApiError ? err.message : "No se pudo cargar la auditoría."));
  }, []);

  return (
    <section>
      <h1>Auditoría de historia clínica</h1>
      <p className="muted">Quién vio o modificó qué, y cuándo. Datos de demostración ficticios.</p>
      {error ? <p className="error">{error}</p> : null}
      <table>
        <thead>
          <tr>
            <th>Cuándo</th>
            <th>Acción</th>
            <th>Recurso</th>
            <th>Actor</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id}>
              <td>{new Date(row.occurredAt).toLocaleString()}</td>
              <td>{labelOf(auditActionLabels, row.action)}</td>
              <td>
                {row.resourceType} · {row.resourceId.slice(0, 8)}
              </td>
              <td>{row.actorUserId.slice(0, 8)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
