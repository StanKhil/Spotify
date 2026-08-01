namespace Spotify.Application.DTOs.Plugin;

public sealed class UpdatePluginSettingsRequest
{
    public string? SettingsJson { get; init; }
}