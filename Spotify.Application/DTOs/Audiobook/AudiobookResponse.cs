using Spotify.Application.DTOs.Author;
namespace Spotify.Application.DTOs.Audiobook;

public sealed record AudiobookResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid? AudioItemId,
    string? AudioUrl,
    string? GenreId,
    DateTime CreatedAt,
    IReadOnlyCollection<AuthorResponse> Authors);
