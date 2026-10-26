namespace Spotify.Application.DTOs.Playback;

public sealed record AudioContentPlaybackResponse(
    Guid AudioContentId,
    string ContentType,
    string Name,
    int DurationSeconds,
    string? StreamUrl,
    DateTimeOffset? ExpiresAtUtc,
    bool IsExternalStream);
