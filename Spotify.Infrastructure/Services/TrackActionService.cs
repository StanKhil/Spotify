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
    private readonly byte _maxHistoryItems = 5;
    private readonly ApplicationContext _context;
    private readonly IFromJamendoToLocalService _fromJamendoToLocalService;
    private readonly IAudioUrlResolver _audioUrlResolver;

    public TrackActionService(
        ApplicationContext context,
        IAudioUrlResolver audioUrlResolver,
        IFromJamendoToLocalService fromJamendoToLocalService)
    {
        _context = context;
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

        var authorContent = track.AuthorContent;

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

        var authorContent = track.AuthorContent;

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
                AuthorContentId = authorContent.Id,
                LikedAt = DateTime.UtcNow
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

        var authorContent = track.AuthorContent;

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

    public async Task<LikedTracksResult> GetLikedTracksAsync(int maxPerPage, 
        int page, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        if (maxPerPage <= 0 || page <= 0)
        {
            return LikedTracksResult.Failure("Invalid pagination parameters.");
        }

        var rawData = await _context.Likes
            .Where(l => l.ApplicationUserId == userId)
            .Where(l => l.AuthorContent.Item is Track)
            .Where(l => l.AuthorContent.Item.DeletedAt == null)
            .OrderByDescending(l => l.LikedAt)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .Select(l => new
            {
                Id = l.AuthorContent.Item.Id,
                Track = l.AuthorContent.Item as Track,
                AuthorNames = l.AuthorContent.Authors.Select(a => a.Author.Name),
                AlbumName = (l.AuthorContent.Item as Track).Album.Name,
                LikedAt = l.LikedAt,
                ExternalId = l.AuthorContent.Item.ExternalContentId
            })
            .ToListAsync(cancellationToken);

        var result = rawData.Select(x => new LikedTrackResponse(
            x.Id,
            x.ExternalId,
            x.Track?.Name ?? string.Empty,
            x.AuthorNames.Any() ? string.Join(", ", x.AuthorNames) : "Unknown Author",
            x.AlbumName ?? "Unknown Album",
            x.LikedAt.ToString("dd.MM.yyyy"),
            x.Track?.DurationSeconds ?? 0
        )).ToList();

        return LikedTracksResult.Success(result);
    }

    public async Task<ListeningHistoryResult> GetListeningHistoryAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            return ListeningHistoryResult.Failure("User not found.");
        }

        var history = await _context.ListeningHistories
            .Where(lh => lh.ApplicationUserId == userId)
            .OrderByDescending(lh => lh.PlayedAt)
            .Include(lh => lh.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(i => (i as Track)!.Album)
            .Include(lh => lh.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(i => (i as Track)!.AudioItem)
            .Include(lh => lh.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(i => (i as Track)!.ImageItem)
            .Include(lh => lh.AuthorContent)
                .ThenInclude(ac => ac.Item)
            .Include(lh => lh.AuthorContent)
                .ThenInclude(ac => ac.Authors)
                    .ThenInclude(a => a.Author)
            .ToListAsync(cancellationToken);

        var uniqueHistory = history
            .Where(h => h.AuthorContent?.Item is Track)
            .GroupBy(h => h.AuthorContent!.Item.Id)
            .Select(g => g.First())
            .Take(5)
            .ToList();

        if (uniqueHistory.Count == 0)
        {
            return ListeningHistoryResult.Failure(
                "No listening history found for the user.");
        }

        var result = new List<ListeningHistoryResponse>();

        foreach (var item in uniqueHistory)
        {
            var track = (Track)item.AuthorContent!.Item;

            var artistName = item.AuthorContent?.Authors
                    .Select(a => a.Author.Name)
                    .FirstOrDefault() ?? "Unknown Artist";

            var albumName = track.Album.Name;

            var audioUrl = await _audioUrlResolver.ResolveAsync(
                track,
                cancellationToken);

            result.Add(new ListeningHistoryResponse(
                item.Id,
                track.Id,
                track.Name,
                artistName,
                track.Album?.Name ?? "Unknown Album",
                track.DurationSeconds.ToString(),
                audioUrl,
                track.ImageItem?.ImageList
            ));
        }

        return ListeningHistoryResult.Success(result);
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
            .Include(x => x.AuthorContent)
                .ThenInclude(ac => ac.Authors)
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