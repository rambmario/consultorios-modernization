import { anamnesisSections } from "./anamnesisSections";

describe("anamnesisSections", () => {
  it("exposes the six long Spanish names instead of abbreviations", () => {
    expect(anamnesisSections.map((section) => section.label)).toEqual([
      "Motivo de consulta",
      "Antecedentes de la enfermedad actual",
      "Antecedentes personales patológicos",
      "Antecedentes familiares",
      "Examen físico",
      "Antecedentes de hábitos"
    ]);
    expect(anamnesisSections.some((section) => /^[A-Z]{2,4}$/.test(section.label))).toBe(false);
  });
});
