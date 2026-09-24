using Spotify.Application.DTOs.Playback;

namespace Spotify.Application.Interfaces;

public interface IPlaybackService
{
    Task<TrackPlaybackResponse?> GetTrackPlaybackAsync(
        Guid trackId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<TrackPlaybackResponse?> GetTrackPlaybackForAdminAsync(
        Guid trackId,
        bool asUser,
        Guid? userId,
        CancellationToken cancellationToken = default);
}