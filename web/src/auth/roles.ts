export const Roles = {
  Admin: "Admin",
  Profesional: "Profesional",
  Recepcion: "Recepcion"
} as const;

export type NavItem = { to: string; label: string };

export function canAccessClinicalRecord(roles: readonly string[]): boolean {
  return roles.includes(Roles.Profesional);
}

export function canAccessAgenda(roles: readonly string[]): boolean {
  return roles.includes(Roles.Profesional) || roles.includes(Roles.Recepcion);
}

export function canAccessAudit(roles: readonly string[]): boolean {
  return roles.includes(Roles.Admin) || roles.includes(Roles.Profesional);
}

export function navItemsFor(roles: readonly string[]): NavItem[] {
  const items: NavItem[] = [{ to: "/patients", label: "Pacientes" }];
  if (canAccessAgenda(roles)) {
    items.push({ to: "/appointments", label: "Agenda" });
  }
  if (canAccessAudit(roles)) {
    items.push({ to: "/audit", label: "Auditoría" });
  }
  return items;
}
