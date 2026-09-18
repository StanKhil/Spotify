namespace Spotify.Application.DTOs.Audiobook;

public sealed record AudiobookActionResponse(
    Guid AudiobookId,
    bool IsLiked);
