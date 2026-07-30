using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Infrastructure.Playback;

namespace Spotify.Infrastructure.Services;

public sealed class PlaybackService : IPlaybackService
{
    private readonly ApplicationContext _context;
    private readonly JamendoApiClient _jamendoApiClient;
    private readonly ILocalPlaybackUrlService _localPlaybackUrlService;
    private readonly PlaybackOptions _playbackOptions;

    public PlaybackService(
        ApplicationContext context,
        JamendoApiClient jamendoApiClient,
        ILocalPlaybackUrlService localPlaybackUrlService,
        PlaybackOptions playbackOptions)
    {
        _context = context;
        _jamendoApiClient = jamendoApiClient;
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
            .FirstOrDefaultAsync(x => x.Id == trackId && x.DeletedAt == null && !x.IsDraft, cancellationToken);

        if (track?.AudioItem is null)
        {
            return null;
        }

        if (track.IsForAdult)
        {
            var userIsAdult = await _context.UserProfiles
                .AnyAsync(x => x.UserId == userId && x.IsAdult && x.DeletedAt == null, cancellationToken);

            if (!userIsAdult)
            {
                return null;
            }
        }

        if (track.AudioItem.Provider == AudioProvider.Jamendo)
        {
            if (string.IsNullOrWhiteSpace(track.AudioItem.ExternalContentId))
            {
                return null;
            }

            var streamUrl = await _jamendoApiClient.GetTrackStreamUrlAsync(
                track.AudioItem.ExternalContentId,
                cancellationToken);

            return streamUrl is null
                ? null
                : new TrackPlaybackResponse(track.Id, track.Name, track.DurationSeconds, streamUrl, null, true);
        }

        if (string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
        {
            return null;
        }

        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_playbackOptions.LocalUrlLifetimeMinutes);
        var localStreamUrl = _localPlaybackUrlService.CreateStreamUrl(track.AudioItem.Id, expiresAtUtc);
        return new TrackPlaybackResponse(track.Id, track.Name, track.DurationSeconds, localStreamUrl, expiresAtUtc, false);
    }
}
