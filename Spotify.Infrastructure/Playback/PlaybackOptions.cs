namespace Spotify.Infrastructure.Playback;

public sealed class PlaybackOptions
{
    public const string SectionName = "Playback";

    public string LocalStorageRoot { get; init; } = "App_Data/audio";
    public int LocalUrlLifetimeMinutes { get; init; } = 10;
    public string SigningKey { get; init; } = string.Empty;
}
