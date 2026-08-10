using Spotify.Application.DTOs.Track;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.Interfaces
{
    public interface ITrackActionService
    {
        Task<TrackActionResponse?> PlayAsync(
            Guid trackId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<TrackActionResponse?> LikeAsync(
            Guid trackId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<TrackActionResponse?> UnlikeAsync(
            Guid trackId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
