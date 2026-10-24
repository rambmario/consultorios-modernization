import type { ClinicalRecordResponse, PatientResponse } from "../api/models";

export const demoPatient: PatientResponse = {
  id: "aaaaaaaa-aaaa-7aaa-8aaa-aaaaaaaaaaaa",
  lastName: "Demo",
  firstName: "Paciente 1",
  nationalId: "20000001",
  phone: "11-4000-0001",
  address: {
    street: "Calle Ficticia",
    number: "100",
    city: "CABA"
  },
  firstConsultationAt: "2026-03-15T10:00:00-03:00",
  coverage: {
    healthPlanId: "bbbbbbbb-bbbb-7bbb-8bbb-bbbbbbbbbbbb",
    healthPlanName: "Obra Social Demo",
    affiliationNumber: "AFF-0001"
  }
};

export const demoPatientTwo: PatientResponse = {
  ...demoPatient,
  id: "cccccccc-cccc-7ccc-8ccc-cccccccccccc",
  lastName: "Demo",
  firstName: "Paciente 2",
  nationalId: "20000002"
};

export const demoClinicalRecord: ClinicalRecordResponse = {
  patient: demoPatient,
  anamnesis: {
    id: "dddddddd-dddd-7ddd-8ddd-dddddddddddd",
    patientId: demoPatient.id,
    chiefComplaint: "Control de rutina ficticio",
    currentIllnessHistory: "Sin eventos actuales de demostración",
    personalPathologicalHistory: "Sin antecedentes personales de demostración",
    familyHistory: "Sin antecedentes familiares de demostración",
    physicalExamination: "Sin hallazgos de demostración",
    habitHistory: "Hábitos de demostración",
    updatedAt: "2026-10-24T15:00:00-03:00"
  },
  progressNotes: [
    {
      id: "eeeeeeee-eeee-7eee-8eee-eeeeeeeeeeee",
      patientId: demoPatient.id,
      professionalId: "ffffffff-ffff-7fff-8fff-ffffffffffff",
      body: "Evolución de demostración ya firmada.",
      signedAt: "2026-10-24T15:30:00-03:00",
      status: "Active"
    }
  ],
  diagnoses: [
    {
      id: "99999999-9999-7999-8999-999999999999",
      patientId: demoPatient.id,
      diagnosisId: "88888888-8888-7888-8888-888888888888",
      diagnosisName: "Diagnóstico de demostración",
      professionalId: "ffffffff-ffff-7fff-8fff-ffffffffffff",
      recordedAt: "2026-10-01T09:00:00-03:00",
      validFrom: "2026-10-01T09:00:00-03:00",
      isCurrent: true
    }
  ]
};
