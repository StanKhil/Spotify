using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

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

        if (track.IsForAdult && !await IsUserAdultAsync(userId, cancellationToken))
        {
            return null;
        }

        return await BuildPlaybackResponseAsync(track, cancellationToken);
    }

    public async Task<TrackPlaybackResponse?> GetTrackPlaybackForAdminAsync(
        Guid trackId,
        bool asUser,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tracks
            .AsNoTracking()
            .Include(x => x.AudioItem)
            .Where(x => x.Id == trackId && x.DeletedAt == null);

        if (asUser)
        {
            query = query.Where(x => !x.IsDraft);
        }

        var track = await query.FirstOrDefaultAsync(cancellationToken);

        if (track?.AudioItem is null)
        {
            return null;
        }

        if (asUser && track.IsForAdult && !await IsUserAdultAsync(userId, cancellationToken))
        {
            return null;
        }

        return await BuildPlaybackResponseAsync(track, cancellationToken);
    }

    private async Task<bool> IsUserAdultAsync(Guid? userId, CancellationToken cancellationToken)
    {
        if (userId is null)
        {
            return false;
        }

        return await _context.UserProfiles.AnyAsync(
            x => x.UserId == userId &&
                 x.IsAdult &&
                 x.DeletedAt == null,
            cancellationToken);
    }

    private async Task<TrackPlaybackResponse?> BuildPlaybackResponseAsync(
        Domain.Entities.Content.Track track,
        CancellationToken cancellationToken)
    {
        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(
            _playbackOptions.LocalUrlLifetimeMinutes);

        string? streamUrl = null;

        switch (track.Provider)
        {
            case AudioProvider.LocalStorage:
                {
                    if (string.IsNullOrWhiteSpace(track.AudioItem!.StorageKey))
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