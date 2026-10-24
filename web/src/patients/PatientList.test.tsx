import { ThemeProvider } from "@mui/material";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import type { ComponentProps } from "react";
import { MemoryRouter } from "react-router-dom";
import { demoPatient, demoPatientTwo } from "../test/fixtures";
import { consultoriosTheme } from "../theme/consultoriosTheme";
import { PatientList } from "./PatientList";

function renderList(overrides: Partial<ComponentProps<typeof PatientList>> = {}) {
  const onSearch = vi.fn();
  const onSearchChange = vi.fn();
  const onPageChange = vi.fn();
  render(
    <ThemeProvider theme={consultoriosTheme}>
      <MemoryRouter>
        <PatientList
          items={[demoPatient, demoPatientTwo]}
          search=""
          onSearchChange={onSearchChange}
          onSearch={onSearch}
          page={1}
          pageSize={10}
          totalCount={25}
          onPageChange={onPageChange}
          {...overrides}
        />
      </MemoryRouter>
    </ThemeProvider>
  );
  return { onSearch, onSearchChange, onPageChange };
}

describe("PatientList", () => {
  it("lists fictitious patients", () => {
    renderList();
    expect(screen.getByRole("link", { name: "Demo, Paciente 1" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Demo, Paciente 2" })).toBeInTheDocument();
    expect(screen.getByText("20000001")).toBeInTheDocument();
  });

  it("submits the search term", async () => {
    const user = userEvent.setup();
    const { onSearch, onSearchChange } = renderList({ search: "Demo" });

    await user.type(
      screen.getByLabelText("Buscar por apellido, nombre o documento"),
      " extra"
    );
    expect(onSearchChange).toHaveBeenCalled();

    await user.click(screen.getByRole("button", { name: "Buscar" }));
    expect(onSearch).toHaveBeenCalledOnce();
  });

  it("moves to the next page", async () => {
    const user = userEvent.setup();
    const { onPageChange } = renderList();

    await user.click(screen.getByRole("button", { name: "Ir a la página siguiente" }));
    expect(onPageChange).toHaveBeenCalledWith(2);
  });
});
