using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Playback;

public sealed class PlaybackService : IPlaybackService
{
    private readonly ApplicationContext _context;
    private readonly ILocalPlaybackUrlService _localPlaybackUrlService;
    private readonly PlaybackOptions _playbackOptions;

    public PlaybackService(
        ApplicationContext context,
        ILocalPlaybackUrlService localPlaybackUrlService,
        PlaybackOptions playbackOptions)
    {
        _context = context;
        _localPlaybackUrlService = localPlaybackUrlService;
        _playbackOptions = playbackOptions;
    }

    public async Task<TrackPlaybackResponse?> GetTrackPlaybackAsync(
        Guid trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks
            .AsNoTracking()
            .Include(x => x.AudioItem)
            .FirstOrDefaultAsync(
                x => x.Id == trackId &&
                     x.DeletedAt == null &&
                     !x.IsDraft,
                cancellationToken);

        if (track?.AudioItem is null)
        {
            return null;
        }

        if (track.IsForAdult)
        {
            var userIsAdult = await _context.UserProfiles
                .AnyAsync(
                    x => x.UserId == userId &&
                         x.IsAdult &&
                         x.DeletedAt == null,
                    cancellationToken);

            if (!userIsAdult)
            {
                return null;
            }
        }

        if (string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
        {
            return null;
        }

        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(
            _playbackOptions.LocalUrlLifetimeMinutes);

        var streamUrl = _localPlaybackUrlService.CreateStreamUrl(
            track.AudioItem.Id,
            expiresAtUtc);

        return new TrackPlaybackResponse(
            track.Id,
            track.Name,
            track.DurationSeconds,
            streamUrl,
            expiresAtUtc,
            false);
    }
}