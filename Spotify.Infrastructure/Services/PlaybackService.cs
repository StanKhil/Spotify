using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;
using System.Diagnostics.Eventing.Reader;

namespace Spotify.Infrastructure.Playback;

public sealed class PlaybackService : IPlaybackService
{
    private readonly ApplicationContext _context;
    private readonly ILocalPlaybackUrlService _localPlaybackUrlService;
    private readonly IJamendoService _jamendoService;
    private readonly PlaybackOptions _playbackOptions;

    public PlaybackService(
        ApplicationContext context,
        ILocalPlaybackUrlService localPlaybackUrlService,
        PlaybackOptions playbackOptions,
        IJamendoService jamendoService)
    {
        _context = context;
        _localPlaybackUrlService = localPlaybackUrlService;
        _playbackOptions = playbackOptions;
        _jamendoService = jamendoService;
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

        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(
            _playbackOptions.LocalUrlLifetimeMinutes);

        string? streamUrl = null;

        switch (track.Provider)
        {
            case AudioProvider.LocalStorage:
                {
                    if (string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
                    {
                        return null;
                    }

                    streamUrl = _localPlaybackUrlService.CreateStreamUrl(
                        track.AudioItem.Id,
                        expiresAtUtc);

                    break;
                }

            case AudioProvider.Jamendo:
                {
                    if (string.IsNullOrWhiteSpace(track.ExternalContentId))
                    {
                        return null;
                    }

                    streamUrl = await _jamendoService.GetTrackStreamUrlAsync(
                        track.ExternalContentId,
                        cancellationToken);

                    break;
                }

            default:
                return null;
        }

        return new TrackPlaybackResponse(
            track.Id,
            track.Name,
            track.DurationSeconds,
            streamUrl,
            expiresAtUtc,
            false);
    }
}