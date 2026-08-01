namespace Spotify.Application.DTOs.SystemSettings;

public sealed class UpdateSystemSettingsRequest
{
    public IReadOnlyDictionary<string, string> Settings { get; init; } = new Dictionary<string, string>();
}