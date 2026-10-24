namespace Consultorios.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }

    Guid CurrentUserIdOrEmpty { get; }

    bool HasUser { get; }
}
