namespace Spotify.Application.DTOs.Audiobook;

public sealed record AudiobookResponseCollection(
    IReadOnlyCollection<AudiobookResponse> Audiobooks,
    int TotalCount,
    int TotalPages);
