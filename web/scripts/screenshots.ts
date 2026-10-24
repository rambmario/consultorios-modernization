/**
 * Capture README screenshots against a local Dev stack.
 * Requires: API on :5088, SPA on :5173, docker Postgres + seed.
 *
 * Usage (from web/): npm run screenshots
 * Password: DEMO_PASSWORD_PROFESIONAL from repo-root .env (never hardcoded).
 */
import { chromium, type Page } from "playwright";
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const webRoot = path.resolve(__dirname, "..");
const repoRoot = path.resolve(webRoot, "..");
const imagesDir = path.join(repoRoot, "docs", "images");
const apiBase = process.env.CONSULTORIOS_API_URL ?? "http://localhost:5088";
const appBase = process.env.CONSULTORIOS_WEB_URL ?? "http://localhost:5173";
const profesionalEmail = "profesional.clinicaa@demo.local";

interface LoginResponse {
  accessToken: string;
}

interface PagedResult<T> {
  items: T[];
}

interface PatientResponse {
  id: string;
}

function loadEnvFile(filePath: string): Record<string, string> {
  if (!fs.existsSync(filePath)) {
    return {};
  }

  const values: Record<string, string> = {};
  for (const line of fs.readFileSync(filePath, "utf8").split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) {
      continue;
    }
    const eq = trimmed.indexOf("=");
    if (eq <= 0) {
      continue;
    }
    const key = trimmed.slice(0, eq).trim();
    let value = trimmed.slice(eq + 1).trim();
    if (
      (value.startsWith('"') && value.endsWith('"')) ||
      (value.startsWith("'") && value.endsWith("'"))
    ) {
      value = value.slice(1, -1);
    }
    values[key] = value;
  }
  return values;
}

function requireProfesionalPassword(): string {
  const fromProcess = process.env.DEMO_PASSWORD_PROFESIONAL?.trim();
  if (fromProcess) {
    return fromProcess;
  }

  const env = loadEnvFile(path.join(repoRoot, ".env"));
  const password = env.DEMO_PASSWORD_PROFESIONAL?.trim();
  if (!password) {
    throw new Error(
      "DEMO_PASSWORD_PROFESIONAL is missing. Set it in the repo-root .env (see .env.example)."
    );
  }
  return password;
}

async function apiJson<T>(url: string, init: RequestInit & { token?: string } = {}): Promise<T> {
  const headers = new Headers(init.headers);
  headers.set("Accept", "application/json");
  if (init.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }
  if (init.token) {
    headers.set("Authorization", `Bearer ${init.token}`);
  }

  const response = await fetch(url, { ...init, headers });
  if (!response.ok) {
    const body = await response.text();
    throw new Error(`${init.method ?? "GET"} ${url} → ${response.status}: ${body}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function firstPatientId(password: string): Promise<string> {
  const login = await apiJson<LoginResponse>(`${apiBase}/api/auth/login`, {
    method: "POST",
    body: JSON.stringify({ email: profesionalEmail, password })
  });
  const page = await apiJson<PagedResult<PatientResponse>>(`${apiBase}/api/patients?pageSize=10`, {
    token: login.accessToken
  });
  const id = page.items[0]?.id;
  if (!id) {
    throw new Error("No fictitious patients in the seed.");
  }
  return id;
}

async function captureLogin(page: Page): Promise<void> {
  await page.goto(`${appBase}/login`, { waitUntil: "networkidle" });
  await page.getByRole("heading", { name: "Ingreso al consultorio" }).waitFor({ state: "visible" });
  await page.getByLabel("Contraseña").waitFor({ state: "visible" });
  await page.waitForTimeout(300);
  await page.screenshot({ path: path.join(imagesDir, "login.png"), fullPage: false });
  console.log("Wrote docs/images/login.png");
}

async function loginUi(page: Page, password: string): Promise<void> {
  await page.getByLabel("Email").fill(profesionalEmail);
  await page.getByLabel("Contraseña").fill(password);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await page.waitForURL(/\/patients/, { timeout: 30_000 });
  await page.getByRole("heading", { name: "Pacientes" }).waitFor({ state: "visible" });
}

async function capturePatients(page: Page): Promise<void> {
  await page.getByRole("table", { name: "Lista de pacientes" }).waitFor({ state: "visible" });
  await page.getByRole("link", { name: /Paciente Demo/ }).first().waitFor({ state: "visible" });
  await page.waitForTimeout(400);
  await page.screenshot({ path: path.join(imagesDir, "patients.png"), fullPage: false });
  console.log("Wrote docs/images/patients.png");
}

async function captureClinicalRecord(page: Page, patientId: string): Promise<void> {
  await page.goto(`${appBase}/patients/${patientId}`, { waitUntil: "networkidle" });
  await page.getByRole("tab", { name: "Historia clínica" }).click();
  await page.getByText("Motivo de consulta").waitFor({ state: "visible" });
  await page.getByText("Antecedentes de la enfermedad actual").waitFor({ state: "visible" });
  await page.waitForTimeout(400);
  await page.screenshot({ path: path.join(imagesDir, "clinical-record.png"), fullPage: true });
  console.log("Wrote docs/images/clinical-record.png");
}

async function main(): Promise<void> {
  const password = requireProfesionalPassword();
  fs.mkdirSync(imagesDir, { recursive: true });

  console.log(`API ${apiBase} · Web ${appBase}`);
  const patientId = await firstPatientId(password);

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

  try {
    await captureLogin(page);
    await loginUi(page, password);
    await capturePatients(page);
    await captureClinicalRecord(page, patientId);
  } finally {
    await browser.close();
  }

  console.log("Screenshots complete.");
}

main().catch((error: unknown) => {
  console.error(error instanceof Error ? error.message : error);
  process.exitCode = 1;
});
