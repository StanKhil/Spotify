namespace Spotify.Application.DTOs.Search;

public sealed record SearchAuthorResponse(
    string Id,
    string Name,
    string Provider,
    string? Bio,
    string? ImageUrl,
    int ContentCount,
    string? WebsiteUrl);
