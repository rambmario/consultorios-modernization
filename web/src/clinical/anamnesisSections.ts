import type { AnamnesisResponse } from "../api/models";

export const anamnesisSections = [
  { key: "chiefComplaint", label: "Motivo de consulta" },
  { key: "currentIllnessHistory", label: "Antecedentes de la enfermedad actual" },
  { key: "personalPathologicalHistory", label: "Antecedentes personales patológicos" },
  { key: "familyHistory", label: "Antecedentes familiares" },
  { key: "physicalExamination", label: "Examen físico" },
  { key: "habitHistory", label: "Antecedentes de hábitos" }
] as const satisfies ReadonlyArray<{ key: keyof AnamnesisResponse; label: string }>;
