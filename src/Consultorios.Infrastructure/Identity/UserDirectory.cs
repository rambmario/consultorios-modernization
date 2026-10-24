using Consultorios.Application.Abstractions;
using Consultorios.Application.Auth;
using Microsoft.AspNetCore.Identity;

namespace Consultorios.Infrastructure.Identity;

public sealed class UserDirectory : IUserDirectory
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantContext _tenant;

    public UserDirectory(UserManager<ApplicationUser> userManager, ITenantContext tenant)
    {
        _userManager = userManager;
        _tenant = tenant;
    }

    public async Task<IReadOnlyList<ProfessionalListItem>> ListProfessionalsAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.GetUsersInRoleAsync(AppRoles.Profesional);
        return users
            .Where(user => user.TenantId == _tenant.TenantId)
            .OrderBy(user => user.Email)
            .Select(user => new ProfessionalListItem(user.Id, user.Email ?? string.Empty))
            .ToList();
    }
}
