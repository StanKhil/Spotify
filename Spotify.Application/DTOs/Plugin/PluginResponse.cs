namespace Spotify.Application.DTOs.Plugin;

public sealed record PluginResponse(
    Guid Id,
    string Name,
    bool IsEnabled,
    string? SettingsJson);