namespace Spotify.Application.DTOs.Auth;

public sealed record RegisterResponse(Guid UserId, string Email, string UserName);
