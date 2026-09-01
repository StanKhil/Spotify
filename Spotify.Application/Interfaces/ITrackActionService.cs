using Spotify.Application.DTOs.Track;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.Interfaces
{
    public interface ITrackActionService
    {
        Task<TrackActionResponse?> PlayAsync(
            string trackId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<TrackActionResponse?> LikeAsync(
            string trackId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<TrackActionResponse?> UnlikeAsync(
            string trackId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<GetLikedTracksResult> GetLikedTracksAsync(int maxPerPage, int page, Guid userId, CancellationToken cancellationToken = default);
    }
}
