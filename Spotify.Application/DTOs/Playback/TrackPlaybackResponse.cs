namespace Spotify.Application.DTOs.Playback;

public sealed record TrackPlaybackResponse(
    Guid TrackId,
    string Name,
    int DurationSeconds,
    string StreamUrl,
    DateTimeOffset? ExpiresAtUtc,
    bool IsExternalStream);
