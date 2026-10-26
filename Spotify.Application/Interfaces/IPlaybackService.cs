using Spotify.Application.DTOs.Playback;

namespace Spotify.Application.Interfaces;

public interface IPlaybackService
{
    Task<string?> GetAudioContentPlaybackAsync(
        Guid audioContentId,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<AudioContentPlaybackResponse?> GetAudioContentPlaybackForAdminAsync(
        Guid audioContentId,
        bool asUser,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<bool> RegisterPlayAsync(Guid trackId, CancellationToken cancellationToken = default);
}
