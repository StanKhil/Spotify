namespace Spotify.Application.DTOs.Plugin;

public sealed class TogglePluginRequest
{
    public bool IsEnabled { get; init; }
}