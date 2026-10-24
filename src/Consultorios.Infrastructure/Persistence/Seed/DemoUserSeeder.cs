using Consultorios.Application.Auth;
using Consultorios.Domain.Tenancy;
using Consultorios.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Consultorios.Infrastructure.Persistence.Seed;

public static class DemoUserSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ConsultoriosDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoUserSeeder");

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminPassword = RequireDemoPassword("DEMO_PASSWORD_ADMIN");
        var profesionalPassword = RequireDemoPassword("DEMO_PASSWORD_PROFESIONAL");
        var recepcionPassword = RequireDemoPassword("DEMO_PASSWORD_RECEPTION");

        var clinicA = await db.Tenants.AsNoTracking().SingleOrDefaultAsync(t => t.Name == "ClinicaA", cancellationToken);
        var clinicB = await db.Tenants.AsNoTracking().SingleOrDefaultAsync(t => t.Name == "ClinicaB", cancellationToken);
        if (clinicA is null || clinicB is null)
        {
            logger.LogWarning("Demo tenants were not found; skipping demo user seed.");
            return;
        }

        await EnsureUserAsync(userManager, logger, clinicA, DemoIds.AdminClinicA, AppRoles.Admin, adminPassword);
        await EnsureUserAsync(
            userManager,
            logger,
            clinicA,
            DemoIds.ProfesionalClinicA,
            AppRoles.Profesional,
            profesionalPassword,
            DemoIds.ProfesionalA);
        await EnsureUserAsync(
            userManager,
            logger,
            clinicA,
            DemoIds.Profesional2ClinicA,
            AppRoles.Profesional,
            profesionalPassword,
            DemoIds.ProfesionalA2);
        await EnsureUserAsync(userManager, logger, clinicA, DemoIds.RecepcionClinicA, AppRoles.Recepcion, recepcionPassword);

        await EnsureUserAsync(userManager, logger, clinicB, DemoIds.AdminClinicB, AppRoles.Admin, adminPassword);
        await EnsureUserAsync(
            userManager,
            logger,
            clinicB,
            DemoIds.ProfesionalClinicB,
            AppRoles.Profesional,
            profesionalPassword,
            DemoIds.ProfesionalB);
        await EnsureUserAsync(userManager, logger, clinicB, DemoIds.RecepcionClinicB, AppRoles.Recepcion, recepcionPassword);
    }

    private static string RequireDemoPassword(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"{variableName} is not set. Copy .env.example to .env (or use user-secrets) and set demo passwords.");
        }

        return value;
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        Tenant tenant,
        string email,
        string role,
        string password,
        Guid? userId = null)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            TenantId = tenant.Id
        };

        if (userId is { } id)
        {
            user.Id = id;
        }

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create demo user '{email}': {errors}");
        }

        await userManager.AddToRoleAsync(user, role);
        logger.LogInformation("Seeded demo user {Email} ({Role}) for tenant {Tenant}.", email, role, tenant.Name);
    }
}
