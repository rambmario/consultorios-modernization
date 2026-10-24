import type { LoginResponse } from "../api/models";

export const TOKEN_STORAGE_KEY = "consultorios.token";
export const SESSION_STORAGE_KEY = "consultorios.session";

export interface StoredSession {
  email: string;
  tenantId: string;
  roles: string[];
}

export function persistSession(login: LoginResponse): StoredSession {
  const session: StoredSession = {
    email: login.email,
    tenantId: login.tenantId,
    roles: login.roles
  };
  sessionStorage.setItem(TOKEN_STORAGE_KEY, login.accessToken);
  sessionStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
  return session;
}

export function readSession(): StoredSession | null {
  const raw = sessionStorage.getItem(SESSION_STORAGE_KEY);
  if (!raw) {
    return null;
  }
  try {
    return JSON.parse(raw) as StoredSession;
  } catch {
    return null;
  }
}

export function clearSession(): void {
  sessionStorage.removeItem(TOKEN_STORAGE_KEY);
  sessionStorage.removeItem(SESSION_STORAGE_KEY);
}
