using Consultorios.Application.Abstractions;

namespace Consultorios.Infrastructure.Identity;

public sealed class AmbientCurrentUser : ICurrentUser
{
    private Guid? _userId;

    public Guid UserId =>
        _userId ?? throw new InvalidOperationException("User context is not set.");

    public Guid CurrentUserIdOrEmpty => _userId ?? Guid.Empty;

    public bool HasUser => _userId.HasValue;

    public void SetUser(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        _userId = userId;
    }

    public void Clear() => _userId = null;
}
