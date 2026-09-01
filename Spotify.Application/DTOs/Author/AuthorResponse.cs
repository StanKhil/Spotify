namespace Spotify.Application.DTOs.Author;

public sealed record AuthorResponse(
    Guid Id,
    string UserName,
    int ContentCount);