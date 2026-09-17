namespace Spotify.Application.DTOs.Audiobook;

public sealed record AudiobookResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    string? GenreId,
    DateTime CreatedAt,
    IReadOnlyCollection<Guid> AuthorIds);