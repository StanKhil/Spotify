using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class TrackActionService : ITrackActionService
{
    private readonly ApplicationContext _context;
    private readonly IJamendoService _jamendoService;
    private readonly IFromJamendoToLocalService _fromJamendoToLocalService;
    private readonly IAudioUrlResolver _audioUrlResolver;

    public TrackActionService(
        ApplicationContext context,
        IJamendoService jamendoService,
        IAudioUrlResolver audioUrlResolver,
        IFromJamendoToLocalService fromJamendoToLocalService)
    {
        _context = context;
        _jamendoService = jamendoService;
        _audioUrlResolver = audioUrlResolver;
        _fromJamendoToLocalService = fromJamendoToLocalService;
    }

    public async Task<TrackActionResponse?> PlayAsync(
        string trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetOrCreateTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        track.PlaysNumber++;

        var history = new ListeningHistory
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = userId,
            AuthorContentId = authorContent.Id,
            ListenedSeconds = 0,
            IsCompleted = false,
            PlayedAt = DateTime.UtcNow
        };

        _context.ListeningHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        var isLiked = await IsLikedAsync(
            userId,
            authorContent.Id,
            cancellationToken);

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            isLiked);
    }

    public async Task<TrackActionResponse?> LikeAsync(
        string trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetOrCreateTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        var alreadyLiked = await IsLikedAsync(
            userId,
            authorContent.Id,
            cancellationToken);

        if (!alreadyLiked)
        {
            _context.Likes.Add(new Like
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                AuthorContentId = authorContent.Id
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            true);
    }

    public async Task<TrackActionResponse?> UnlikeAsync(
        string trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetOrCreateTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        var like = await _context.Likes
            .FirstOrDefaultAsync(
                x => x.ApplicationUserId == userId &&
                     x.AuthorContentId == authorContent.Id,
                cancellationToken);

        if (like is not null)
        {
            _context.Likes.Remove(like);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            false);
    }

    public async Task<GetLikedTracksResult> GetLikedTracksAsync(
    int maxPerPage,
    int page,
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        if (maxPerPage <= 0 || page <= 0)
        {
            return GetLikedTracksResult.Failure(
                "Invalid pagination parameters.");
        }

        var likedTracksQuery = _context.Likes
            .Where(l => l.ApplicationUserId == userId)
            .Select(l => l.AuthorContent.Item)
            .OfType<Track>()
            .Where(t =>
                t.DeletedAt == null &&
                !t.IsDraft);

        var totalLikedTracks = await likedTracksQuery
            .CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            (double)totalLikedTracks / maxPerPage);

        var tracks = await likedTracksQuery
            .Include(t => t.AudioItem)
            .Include(t => t.TrackTags)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .ToListAsync(cancellationToken);

        var trackResponses = new List<TrackResponse>();

        foreach (var track in tracks)
        {
            var audioUrl = await _audioUrlResolver.ResolveAsync(
            track,
            cancellationToken);

            trackResponses.Add(new TrackResponse(
                track.Id,
                track.Name,
                track.Description,
                track.DurationSeconds,
                track.AlbumId,
                track.MoodId,
                track.GenreId,
                track.PlaysNumber,
                track.IsAdult,
                track.IsDraft,
                track.AudioItemId,
                track.ImageItemId,
                track.TrackTags
                    .Select(x => x.TagId)
                    .ToList(),
                track.CreatedAt,
                audioUrl
            ));
        }

        return GetLikedTracksResult.Success(
            new TrackResponseCollection(
                trackResponses,
                totalLikedTracks,
                totalPages));
    }

    private async Task<Track?> GetOrCreateTrackAsync(
    string trackId,
    CancellationToken cancellationToken)
    {
        if (Guid.TryParse(trackId, out var localTrackId))
        {
            return await GetLocalTrackAsync(
                localTrackId,
                cancellationToken);
        }

        if (_fromJamendoToLocalService.IsJamendoId(trackId))
        {
            return await _fromJamendoToLocalService.GetOrCreateJamendoTrackAsync(
                trackId,
                cancellationToken);
        }

        return null;
    }

    private async Task<Track?> GetLocalTrackAsync(
        Guid trackId,
        CancellationToken cancellationToken)
    {
        return await _context.Tracks
            .Include(x => x.Authors)
            .FirstOrDefaultAsync(
                x => x.Id == trackId &&
                     x.DeletedAt == null &&
                     !x.IsDraft,
                cancellationToken);
    }

    private async Task<bool> IsLikedAsync(
        Guid userId,
        Guid authorContentId,
        CancellationToken cancellationToken)
    {
        return await _context.Likes
            .AnyAsync(
                x => x.ApplicationUserId == userId &&
                     x.AuthorContentId == authorContentId,
                cancellationToken);
    }
}