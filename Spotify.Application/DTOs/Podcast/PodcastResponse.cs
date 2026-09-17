namespace Spotify.Application.DTOs.Podcast;

public sealed record PodcastResponse(
    Guid Id,
    string Name,
    string Description,
    int EpisodesCount,
    IReadOnlyCollection<Guid> AuthorIds,
    DateTime CreatedAt);