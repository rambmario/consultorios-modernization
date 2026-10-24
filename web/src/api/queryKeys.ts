export const queryKeys = {
  patients: {
    all: ["patients"] as const,
    list: (search: string, page: number, pageSize: number) =>
      ["patients", "list", { search, page, pageSize }] as const,
    detail: (id: string) => ["patients", "detail", id] as const
  },
  clinicalRecord: (patientId: string) => ["clinical-record", patientId] as const,
  appointments: ["appointments"] as const,
  professionals: ["professionals"] as const,
  audits: ["clinical-audits"] as const
};
