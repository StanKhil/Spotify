namespace Spotify.Application.DTOs.Auth;

public sealed record AuthenticationResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string UserName,
    string? Description,
    Guid? AvatarImageId,
    Guid? CoverImageId);
