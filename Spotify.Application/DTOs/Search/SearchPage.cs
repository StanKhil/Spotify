namespace Spotify.Application.DTOs.Search;

public sealed record SearchPage<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int TotalPages);
