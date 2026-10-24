import { clearSession, persistSession, readSession, SESSION_STORAGE_KEY, TOKEN_STORAGE_KEY } from "./session";

describe("session storage", () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it("persists the token and roles, then clears them", () => {
    persistSession({
      accessToken: "token-demo",
      expiresAt: "2026-10-24T15:00:00Z",
      tenantId: "11111111-1111-7111-8111-111111111111",
      email: "profesional.clinicaa@demo.local",
      roles: ["Profesional"]
    });

    expect(sessionStorage.getItem(TOKEN_STORAGE_KEY)).toBe("token-demo");
    expect(readSession()).toEqual({
      email: "profesional.clinicaa@demo.local",
      tenantId: "11111111-1111-7111-8111-111111111111",
      roles: ["Profesional"]
    });

    clearSession();
    expect(sessionStorage.getItem(SESSION_STORAGE_KEY)).toBeNull();
    expect(readSession()).toBeNull();
  });
});
