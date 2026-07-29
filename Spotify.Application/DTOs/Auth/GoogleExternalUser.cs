namespace Spotify.Application.DTOs.Auth;

public sealed record GoogleExternalUser(
    string ProviderKey,
    string Email,
    string DisplayName);
