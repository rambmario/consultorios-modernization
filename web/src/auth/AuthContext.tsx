import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import { api } from "../api/client";
import type { LoginResponse } from "../api/models";
import { clearSession, persistSession, readSession, type StoredSession } from "./session";

interface AuthContextValue {
  session: StoredSession | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<StoredSession | null>(() => readSession());

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      async login(email, password) {
        const response = await api<LoginResponse>("/api/auth/login", {
          method: "POST",
          body: JSON.stringify({ email, password })
        });
        setSession(persistSession(response));
      },
      logout() {
        clearSession();
        setSession(null);
      }
    }),
    [session]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider.");
  }
  return context;
}
