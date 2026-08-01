namespace Spotify.Application.DTOs.Audiobook;

public sealed record AudiobookResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid AuthorContentId,
    string? GenreId,
    DateTime CreatedAt);