import { api } from "./client";
import { TOKEN_STORAGE_KEY } from "../auth/session";

describe("api client", () => {
  beforeEach(() => {
    sessionStorage.clear();
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        text: async () => JSON.stringify({ ok: true })
      })
    );
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("attaches the bearer token from sessionStorage", async () => {
    sessionStorage.setItem(TOKEN_STORAGE_KEY, "jwt-demo");
    await api("/api/patients");

    expect(fetch).toHaveBeenCalledOnce();
    const [, init] = vi.mocked(fetch).mock.calls[0];
    const headers = new Headers(init?.headers);
    expect(headers.get("Authorization")).toBe("Bearer jwt-demo");
  });
});
