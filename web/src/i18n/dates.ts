const buenosAires = "America/Argentina/Buenos_Aires";

function partsOf(date: Date): Record<string, string> {
  const formatter = new Intl.DateTimeFormat("es-AR", {
    timeZone: buenosAires,
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false
  });
  return Object.fromEntries(formatter.formatToParts(date).map((part) => [part.type, part.value]));
}

/** Clinical UI timestamps: `dd/MM/yyyy HH:mm` in es-AR (Buenos Aires). */
export function formatDateTime(value: string | Date | null | undefined): string {
  if (value == null || value === "") {
    return "—";
  }
  const date = typeof value === "string" ? new Date(value) : value;
  if (Number.isNaN(date.getTime())) {
    return "—";
  }
  const parts = partsOf(date);
  return `${parts.day}/${parts.month}/${parts.year} ${parts.hour}:${parts.minute}`;
}
