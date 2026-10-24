using Npgsql;

namespace Consultorios.Infrastructure.Persistence;

public static class PersistenceErrors
{
    public static bool IsAppointmentExclusion(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres
                && postgres.SqlState == PostgresErrorCodes.ExclusionViolation)
            {
                return true;
            }
        }

        return false;
    }
}
