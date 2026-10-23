namespace Consultorios.Domain.Patients;

/// <summary>
/// Long names for anamnesis sections. Abbreviations (MC, AEA, …) are assumed
/// interpretations confirmed by the owner; see docs/legacy/owner-decisions.md.
/// </summary>
public static class AnamnesisSections
{
    public const string ChiefComplaint = "Motivo de consulta";
    public const string CurrentIllnessHistory = "Antecedentes de la enfermedad actual";
    public const string PersonalPathologicalHistory = "Antecedentes personales patológicos";
    public const string FamilyHistory = "Antecedentes familiares";
    public const string PhysicalExamination = "Examen físico";
    public const string HabitHistory = "Antecedentes de hábitos";
}
