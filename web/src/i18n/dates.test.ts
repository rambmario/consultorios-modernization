import { formatDateTime } from "./dates";

describe("formatDateTime", () => {
  it("formats a Buenos Aires instant as dd/MM/yyyy HH:mm", () => {
    expect(formatDateTime("2026-10-24T15:30:00-03:00")).toBe("24/10/2026 15:30");
  });

  it("returns an em dash for empty or invalid values", () => {
    expect(formatDateTime(null)).toBe("—");
    expect(formatDateTime("")).toBe("—");
    expect(formatDateTime("not-a-date")).toBe("—");
  });
});
