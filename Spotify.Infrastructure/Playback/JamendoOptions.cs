namespace Spotify.Infrastructure.Playback;

public sealed class JamendoOptions
{
    public const string SectionName = "Jamendo";

    public string ClientId { get; init; } = string.Empty;

    public string BaseUrl { get; init; } = "https://api.jamendo.com/v3.0/";

    public string AudioFormat { get; init; } = "mp32";

    public int DefaultSearchLimit { get; init; } = 20;
}
