namespace Spotify.Application.DTOs.Plugin;

public sealed class CreatePluginRequest
{
    public string Name { get; init; } = null!;
    public bool IsEnabled { get; init; }
    public string? SettingsJson { get; init; }
}