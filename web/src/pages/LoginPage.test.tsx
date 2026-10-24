import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { renderWithProviders } from "../test/render";
import { LoginPage } from "./LoginPage";

describe("LoginPage", () => {
  it("rejects a short password without calling the API", async () => {
    const user = userEvent.setup();
    const fetchMock = vi.fn();
    vi.stubGlobal("fetch", fetchMock);

    renderWithProviders(<LoginPage />);

    await user.clear(screen.getByLabelText("Contraseña"));
    await user.type(screen.getByLabelText("Contraseña"), "short");
    await user.click(screen.getByRole("button", { name: "Ingresar" }));

    expect(await screen.findByText(/al menos 8 caracteres/)).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
    vi.unstubAllGlobals();
  });
});
