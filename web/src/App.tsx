import { Navigate, Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { Roles } from "./auth/roles";
import { Shell } from "./layout/Shell";
import { AppointmentsPage } from "./pages/AppointmentsPage";
import { AuditPage } from "./pages/AuditPage";
import { ClinicalRecordPage } from "./pages/ClinicalRecordPage";
import { LoginPage } from "./pages/LoginPage";
import { PatientDetailPage } from "./pages/PatientDetailPage";
import { PatientsPage } from "./pages/PatientsPage";

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<Shell />}>
          <Route path="/patients" element={<PatientsPage />} />
          <Route path="/patients/:id" element={<PatientDetailPage />} />
          <Route element={<ProtectedRoute roles={[Roles.Profesional, Roles.Recepcion]} />}>
            <Route path="/appointments" element={<AppointmentsPage />} />
          </Route>
          <Route element={<ProtectedRoute roles={[Roles.Profesional]} />}>
            <Route path="/patients/:id/clinical-record" element={<ClinicalRecordPage />} />
          </Route>
          <Route element={<ProtectedRoute roles={[Roles.Admin, Roles.Profesional]} />}>
            <Route path="/audit" element={<AuditPage />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/patients" replace />} />
    </Routes>
  );
}
