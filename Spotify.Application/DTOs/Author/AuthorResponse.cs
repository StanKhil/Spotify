namespace Spotify.Application.DTOs.Author;

public sealed record AuthorResponse(
    Guid Id,
    string Email,
    string UserName,
    int ContentCount);