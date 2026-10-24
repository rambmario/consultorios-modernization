import { ThemeProvider } from "@mui/material";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { Roles } from "../auth/roles";
import { demoClinicalRecord, demoPatient } from "../test/fixtures";
import { consultoriosTheme } from "../theme/consultoriosTheme";
import { PatientFile } from "./PatientFile";

function renderFile(roles: string[]) {
  const onOpenClinicalTab = vi.fn();
  render(
    <ThemeProvider theme={consultoriosTheme}>
      <MemoryRouter>
        <PatientFile
          patient={demoPatient}
          roles={roles}
          clinicalRecord={demoClinicalRecord}
          onOpenClinicalTab={onOpenClinicalTab}
        />
      </MemoryRouter>
    </ThemeProvider>
  );
  return { onOpenClinicalTab };
}

describe("PatientFile", () => {
  it("shows the clinical-record tab only for Profesional", async () => {
    const user = userEvent.setup();
    const { onOpenClinicalTab } = renderFile([Roles.Profesional]);

    expect(screen.getByRole("tab", { name: "Ficha" })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Historia clínica" })).toBeInTheDocument();

    await user.click(screen.getByRole("tab", { name: "Historia clínica" }));
    expect(onOpenClinicalTab).toHaveBeenCalledOnce();
    expect(screen.getByText("Motivo de consulta.", { exact: false })).toBeInTheDocument();
  });

  it("hides the clinical-record tab for Recepción", () => {
    renderFile([Roles.Recepcion]);

    expect(screen.getByRole("tab", { name: "Ficha" })).toBeInTheDocument();
    expect(screen.queryByRole("tab", { name: "Historia clínica" })).not.toBeInTheDocument();
    expect(
      screen.getByText(/La historia clínica solo la abre un profesional/i)
    ).toBeInTheDocument();
    expect(screen.queryByText("Motivo de consulta.", { exact: false })).not.toBeInTheDocument();
  });

  it("hides the clinical-record tab for Admin", () => {
    renderFile([Roles.Admin]);

    expect(screen.getByRole("tab", { name: "Ficha" })).toBeInTheDocument();
    expect(screen.queryByRole("tab", { name: "Historia clínica" })).not.toBeInTheDocument();
    expect(screen.getByText("15/03/2026 10:00")).toBeInTheDocument();
  });
});
