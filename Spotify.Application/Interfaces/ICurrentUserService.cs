namespace Spotify.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Jti { get; }
    DateTime? ExpiresAtUtc { get; }
}