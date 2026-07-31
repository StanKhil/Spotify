namespace Spotify.Application.DTOs.SystemSettings;

public sealed record SystemSettingsResponse(
    IReadOnlyDictionary<string, string> Settings);