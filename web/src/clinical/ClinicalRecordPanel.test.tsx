import { ThemeProvider } from "@mui/material";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import type { ComponentProps } from "react";
import { demoClinicalRecord } from "../test/fixtures";
import { consultoriosTheme } from "../theme/consultoriosTheme";
import { anamnesisSections } from "./anamnesisSections";
import { ClinicalRecordPanel } from "./ClinicalRecordPanel";

function renderPanel(overrides: Partial<ComponentProps<typeof ClinicalRecordPanel>> = {}) {
  const onSign = vi.fn();
  render(
    <ThemeProvider theme={consultoriosTheme}>
      <ClinicalRecordPanel record={demoClinicalRecord} onSign={onSign} {...overrides} />
    </ThemeProvider>
  );
  return { onSign };
}

describe("ClinicalRecordPanel", () => {
  it("hides the clinical record from Recepción", () => {
    renderPanel({ hidden: true, record: null });

    expect(screen.getByText(/no está disponible para Recepción/i)).toBeInTheDocument();
    expect(screen.queryByText("Motivo de consulta.", { exact: false })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Firmar" })).not.toBeInTheDocument();
  });

  it("shows the long anamnesis section names and a formatted signature date", () => {
    renderPanel();

    for (const section of anamnesisSections) {
      expect(screen.getByText(`${section.label}.`)).toBeInTheDocument();
    }
    expect(screen.getByText(/24\/10\/2026 15:30/)).toBeInTheDocument();
  });

  it("signs a new progress note", async () => {
    const user = userEvent.setup();
    const { onSign } = renderPanel();

    await user.type(
      screen.getByRole("textbox", { name: /Nueva evolución \(queda firmada\)/ }),
      "Evolución de demostración nueva."
    );
    await user.click(screen.getByRole("button", { name: "Firmar" }));

    expect(onSign).toHaveBeenCalledWith("Evolución de demostración nueva.");
  });
});
