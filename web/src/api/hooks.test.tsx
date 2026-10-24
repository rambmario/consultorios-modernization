import { QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { api } from "./client";
import { useCreatePatient, usePatientsQuery } from "./hooks";
import { queryKeys } from "./queryKeys";
import { createTestQueryClient } from "../test/render";

vi.mock("./client", () => ({
  api: vi.fn()
}));

const mockedApi = vi.mocked(api);

function wrapper(queryClient = createTestQueryClient()) {
  return {
    queryClient,
    Wrapper({ children }: { children: ReactNode }) {
      return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
    }
  };
}

describe("patient query hooks", () => {
  beforeEach(() => {
    mockedApi.mockReset();
  });

  it("loads a patient page through the query cache", async () => {
    mockedApi.mockResolvedValue({
      items: [],
      page: 1,
      pageSize: 10,
      totalCount: 0
    });
    const { Wrapper } = wrapper();
    const { result } = renderHook(() => usePatientsQuery("Demo", 1), { wrapper: Wrapper });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(mockedApi).toHaveBeenCalledWith("/api/patients?page=1&pageSize=10&search=Demo");
  });

  it("invalidates the patient list after a successful save", async () => {
    mockedApi.mockResolvedValue({
      id: "aaaaaaaa-aaaa-7aaa-8aaa-aaaaaaaaaaaa",
      lastName: "Demo",
      firstName: "Paciente 1",
      nationalId: "20000001",
      address: {}
    });
    const { queryClient, Wrapper } = wrapper();
    const invalidate = vi.spyOn(queryClient, "invalidateQueries");
    const { result } = renderHook(() => useCreatePatient(), { wrapper: Wrapper });

    await result.current.mutateAsync({
      lastName: "Demo",
      firstName: "Paciente 1",
      nationalId: "20000001"
    });

    expect(invalidate).toHaveBeenCalledWith({ queryKey: queryKeys.patients.all });
  });
});
