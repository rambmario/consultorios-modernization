namespace Consultorios.Application.Abstractions;

public interface IUserDirectory
{
    Task<IReadOnlyList<ProfessionalListItem>> ListProfessionalsAsync(CancellationToken cancellationToken = default);
}

public sealed record ProfessionalListItem(Guid Id, string Email);
