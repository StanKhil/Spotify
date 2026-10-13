namespace Spotify.Application.DTOs.Author;

public sealed record AuthorResponse(
    Guid Id,
    string Name,
    int MonthList,
    string? Bio,
    string? ImageUrl,
    int ContentCount);