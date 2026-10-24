export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  tenantId: string;
  email: string;
  roles: string[];
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface AddressDto {
  street?: string | null;
  number?: string | null;
  apartment?: string | null;
  city?: string | null;
  province?: string | null;
  postalCode?: string | null;
}

export interface CoverageDto {
  healthPlanId: string;
  healthPlanName: string;
  affiliationNumber: string;
}

export interface PatientResponse {
  id: string;
  lastName: string;
  firstName: string;
  nationalId: string;
  phone?: string | null;
  address: AddressDto;
  firstConsultationAt?: string | null;
  coverage?: CoverageDto | null;
}

export interface CreatePatientRequest {
  lastName: string;
  firstName: string;
  nationalId: string;
  phone?: string | null;
}

export interface ProfessionalListItem {
  id: string;
  email: string;
}

export interface AppointmentResponse {
  id: string;
  patientId: string;
  professionalId: string;
  startsAt: string;
  endsAt: string;
  status: number | string;
}

export interface CreateAppointmentRequest {
  patientId: string;
  professionalId: string;
  startsAt: string;
  endsAt: string;
}

export interface AnamnesisResponse {
  id: string;
  patientId: string;
  chiefComplaint: string;
  currentIllnessHistory: string;
  personalPathologicalHistory: string;
  familyHistory: string;
  physicalExamination: string;
  habitHistory: string;
  updatedAt: string;
}

export interface ProgressNoteResponse {
  id: string;
  patientId: string;
  professionalId: string;
  body: string;
  signedAt: string;
  status: number | string;
  correctsNoteId?: string | null;
  cancellationReason?: string | null;
}

export interface PatientDiagnosisResponse {
  id: string;
  patientId: string;
  diagnosisId: string;
  diagnosisName: string;
  professionalId: string;
  recordedAt: string;
  validFrom: string;
  validTo?: string | null;
  isCurrent: boolean;
}

export interface ClinicalRecordResponse {
  patient: PatientResponse;
  anamnesis: AnamnesisResponse;
  progressNotes: ProgressNoteResponse[];
  diagnoses: PatientDiagnosisResponse[];
}

export interface ClinicalAuditResponse {
  id: string;
  actorUserId: string;
  patientId: string;
  action: number | string;
  resourceType: string;
  resourceId: string;
  occurredAt: string;
}

export const appointmentStatusLabels: Record<string, string> = {
  "0": "Programado",
  "1": "Confirmado",
  "2": "Atendido",
  "3": "Cancelado",
  "4": "Ausente",
  Scheduled: "Programado",
  Confirmed: "Confirmado",
  Attended: "Atendido",
  Cancelled: "Cancelado",
  NoShow: "Ausente"
};

export const noteStatusLabels: Record<string, string> = {
  "0": "Activa",
  "1": "Anulada",
  "2": "Reemplazada",
  Active: "Activa",
  Cancelled: "Anulada",
  Superseded: "Reemplazada"
};

export const auditActionLabels: Record<string, string> = {
  "0": "Vista",
  "1": "Alta",
  "2": "Edición",
  "3": "Anulación",
  "4": "Corrección",
  Viewed: "Vista",
  Created: "Alta",
  Updated: "Edición",
  Cancelled: "Anulación",
  Corrected: "Corrección"
};

export function labelOf(map: Record<string, string>, value: number | string): string {
  return map[String(value)] ?? String(value);
}
