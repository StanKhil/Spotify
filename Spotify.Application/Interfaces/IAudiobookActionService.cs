using Spotify.Application.DTOs.Audiobook;

namespace Spotify.Application.Interfaces;

public interface IAudiobookActionService
{
    Task<AudiobookActionResponse?> PlayAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<AudiobookActionResponse?> LikeAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<AudiobookActionResponse?> UnlikeAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<GetLikedAudiobooksResult> GetLikedAudiobooksAsync(
        int maxPerPage,
        int page,
        Guid userId,
        CancellationToken cancellationToken = default);
}
