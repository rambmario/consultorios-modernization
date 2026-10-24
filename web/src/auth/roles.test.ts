import { canAccessAgenda, canAccessAudit, canAccessClinicalRecord, navItemsFor, Roles } from "./roles";

describe("role matrix", () => {
  it("lets only Profesional open the clinical record", () => {
    expect(canAccessClinicalRecord([Roles.Profesional])).toBe(true);
    expect(canAccessClinicalRecord([Roles.Recepcion])).toBe(false);
    expect(canAccessClinicalRecord([Roles.Admin])).toBe(false);
  });

  it("hides agenda from Admin and audit from Recepción", () => {
    expect(canAccessAgenda([Roles.Admin])).toBe(false);
    expect(canAccessAgenda([Roles.Recepcion])).toBe(true);
    expect(canAccessAudit([Roles.Recepcion])).toBe(false);
    expect(canAccessAudit([Roles.Admin])).toBe(true);
  });

  it("builds Spanish nav items from roles", () => {
    expect(navItemsFor([Roles.Recepcion]).map((item) => item.to)).toEqual(["/patients", "/appointments"]);
    expect(navItemsFor([Roles.Admin]).map((item) => item.to)).toEqual(["/patients", "/audit"]);
    expect(navItemsFor([Roles.Profesional]).map((item) => item.to)).toEqual([
      "/patients",
      "/appointments",
      "/audit"
    ]);
  });
});
