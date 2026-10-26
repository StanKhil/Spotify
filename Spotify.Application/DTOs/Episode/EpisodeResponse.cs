namespace Spotify.Application.DTOs.Episode;

public sealed record EpisodeResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid? PodcastId,
    Guid? AudiobookId,
    int? SeqNumber,
    string? AudioUrl,
    Guid? ImageItemId,
    DateTime CreatedAt);