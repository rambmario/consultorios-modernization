import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "./client";
import type {
  AppointmentResponse,
  ClinicalAuditResponse,
  ClinicalRecordResponse,
  CreateAppointmentRequest,
  CreatePatientRequest,
  PagedResult,
  PatientResponse,
  ProfessionalListItem,
  ProgressNoteResponse
} from "./models";
import { queryKeys } from "./queryKeys";

export function usePatientsQuery(search: string, page: number, pageSize = 10) {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize)
  });
  if (search.trim()) {
    params.set("search", search.trim());
  }

  return useQuery({
    queryKey: queryKeys.patients.list(search.trim(), page, pageSize),
    queryFn: () => api<PagedResult<PatientResponse>>(`/api/patients?${params}`)
  });
}

export function usePatientQuery(id: string | undefined) {
  return useQuery({
    queryKey: queryKeys.patients.detail(id ?? ""),
    queryFn: () => api<PatientResponse>(`/api/patients/${id}`),
    enabled: Boolean(id)
  });
}

export function useCreatePatient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreatePatientRequest) =>
      api<PatientResponse>("/api/patients", {
        method: "POST",
        body: JSON.stringify(body)
      }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.patients.all });
    }
  });
}

export function useClinicalRecordQuery(patientId: string | undefined, enabled = true) {
  return useQuery({
    queryKey: queryKeys.clinicalRecord(patientId ?? ""),
    queryFn: () => api<ClinicalRecordResponse>(`/api/patients/${patientId}/clinical-record`),
    enabled: Boolean(patientId) && enabled
  });
}

export function useSignProgressNote(patientId: string | undefined) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: string) =>
      api<ProgressNoteResponse>(`/api/patients/${patientId}/progress-notes`, {
        method: "POST",
        body: JSON.stringify({ body })
      }),
    onSuccess: () => {
      if (!patientId) {
        return;
      }
      void queryClient.invalidateQueries({ queryKey: queryKeys.clinicalRecord(patientId) });
      void queryClient.invalidateQueries({ queryKey: queryKeys.audits });
    }
  });
}

export function useAppointmentsQuery() {
  return useQuery({
    queryKey: queryKeys.appointments,
    queryFn: () => api<PagedResult<AppointmentResponse>>("/api/appointments?pageSize=50")
  });
}

export function useProfessionalsQuery() {
  return useQuery({
    queryKey: queryKeys.professionals,
    queryFn: () => api<ProfessionalListItem[]>("/api/professionals")
  });
}

export function useCreateAppointment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateAppointmentRequest) =>
      api<AppointmentResponse>("/api/appointments", {
        method: "POST",
        body: JSON.stringify(body)
      }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.appointments });
    }
  });
}

export function useAuditsQuery() {
  return useQuery({
    queryKey: queryKeys.audits,
    queryFn: () => api<ClinicalAuditResponse[]>("/api/clinical-audits")
  });
}
