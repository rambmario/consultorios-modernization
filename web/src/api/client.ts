import { TOKEN_STORAGE_KEY } from "../auth/session";

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export function readStoredToken(): string | null {
  return sessionStorage.getItem(TOKEN_STORAGE_KEY);
}

export async function api<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers);
  const token = readStoredToken();
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }
  if (init.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  const response = await fetch(path, { ...init, headers });
  if (response.status === 204) {
    return undefined as T;
  }

  const text = await response.text();
  const payload = text.length === 0 ? null : JSON.parse(text);

  if (!response.ok) {
    const detail =
      payload?.detail ??
      payload?.title ??
      (response.status === 401 ? "No autenticado." : `Error ${response.status}`);
    throw new ApiError(response.status, String(detail));
  }

  return payload as T;
}
