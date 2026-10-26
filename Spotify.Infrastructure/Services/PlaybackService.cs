using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
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

    public async Task<string?> GetAudioContentPlaybackAsync(
        Guid audioContentId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var audioContent = await _context.Set<AudioContent>()
            .AsNoTracking()
            .Include(x => x.AudioItem)
            .FirstOrDefaultAsync(
                x => x.Id == audioContentId && x.DeletedAt == null,
                cancellationToken);

        if (audioContent?.AudioItem is null ||
            audioContent is Track { IsDraft: true })
        {
            return null;
        }

        if (audioContent.IsForAdult && !await IsUserAdultAsync(userId, cancellationToken))
        {
            return null;
        }

        var playback = await BuildPlaybackResponseAsync(audioContent, cancellationToken);
        return playback?.StreamUrl;
    }

    public async Task<AudioContentPlaybackResponse?> GetAudioContentPlaybackForAdminAsync(
        Guid audioContentId,
        bool asUser,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<AudioContent>()
            .AsNoTracking()
            .Include(x => x.AudioItem)
            .Where(x => x.Id == audioContentId && x.DeletedAt == null);

        var audioContent = await query.FirstOrDefaultAsync(cancellationToken);

        if (audioContent?.AudioItem is null ||
            (asUser && audioContent is Track { IsDraft: true }))
        {
            return null;
        }

        if (asUser && audioContent.IsForAdult && !await IsUserAdultAsync(userId, cancellationToken))
        {
            return null;
        }

        return await BuildPlaybackResponseAsync(audioContent, cancellationToken);
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

    private async Task<AudioContentPlaybackResponse?> BuildPlaybackResponseAsync(
        AudioContent audioContent,
        CancellationToken cancellationToken)
    {
        DateTimeOffset? expiresAtUtc = null;
        string? streamUrl = null;

        switch (audioContent.Provider)
        {
            case AudioProvider.LocalStorage:
                {
                    if (string.IsNullOrWhiteSpace(audioContent.AudioItem!.StorageKey))
                    {
                        return null;
                    }

                    expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(
                        _playbackOptions.LocalUrlLifetimeMinutes);

                    streamUrl = _localPlaybackUrlService.CreateStreamUrl(
                        audioContent.AudioItem.Id,
                        expiresAtUtc.Value);

                    break;
                }

            case AudioProvider.Jamendo:
                {
                    if (string.IsNullOrWhiteSpace(audioContent.ExternalContentId))
                    {
                        return null;
                    }

                    streamUrl = await _jamendoService.GetTrackStreamUrlAsync(
                        audioContent.ExternalContentId,
                        cancellationToken);

                    break;
                }

            default:
                return null;
        }

        return new AudioContentPlaybackResponse(
            audioContent.Id,
            GetContentType(audioContent),
            audioContent.Name,
            audioContent.DurationSeconds,
            streamUrl,
            expiresAtUtc,
            audioContent.Provider == AudioProvider.Jamendo);
    }
    public async Task<bool> RegisterPlayAsync(
    Guid trackId, CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks
            .FirstOrDefaultAsync(x => x.Id == trackId && x.DeletedAt == null, cancellationToken);

        if (track is null)
        {
            return false;
        }

        track.PlaysNumber++;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string GetContentType(AudioContent content) => content switch
    {
        Track => "Track",
        Episode => "Episode",
        Audiobook => "Audiobook",
        _ => "AudioContent"
    };
}
