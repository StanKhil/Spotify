using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
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
    private readonly IJamendoService _jamendoService;

    public TrackActionService(
        ApplicationContext context,
        IAudioUrlResolver audioUrlResolver,
        IFromJamendoToLocalService fromJamendoToLocalService,
        IJamendoService jamendoService)
    {
        _context = context;
        _audioUrlResolver = audioUrlResolver;
        _fromJamendoToLocalService = fromJamendoToLocalService;
        _jamendoService = jamendoService;
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
            var lastOrder = await GetLastTrackOrderAsync(userId, cancellationToken);

            _context.Likes.Add(new Like
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                AuthorContentId = authorContent.Id,
                LikedAt = DateTime.UtcNow,
                Order = lastOrder + 1
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
            var removedOrder = like.Order;
            _context.Likes.Remove(like);

            var followingTrackLikes = await GetTrackLikesQuery(userId)
                .Where(x => x.Id != like.Id && x.Order > removedOrder)
                .ToListAsync(cancellationToken);

            foreach (var followingLike in followingTrackLikes)
            {
                followingLike.Order--;
            }

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

        var likes = await _context.Likes
            .Where(l => l.ApplicationUserId == userId)
            .Where(l => l.AuthorContent.Item is Track)
            .Where(l => l.AuthorContent.Item.DeletedAt == null)
            .OrderBy(l => l.Order)
            .ThenByDescending(l => l.LikedAt)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .Include(l => l.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(item => (item as Track)!.AudioItem)
            .Include(l => l.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(item => (item as Track)!.ImageItem)
            .Include(l => l.AuthorContent)
                .ThenInclude(ac => ac.Item)
                    .ThenInclude(item => (item as Track)!.Album)
            .Include(l => l.AuthorContent)
                .ThenInclude(ac => ac.Authors)
                    .ThenInclude(aca => aca.Author)
            .ToListAsync(cancellationToken);

        var result = new List<LikedTrackResponse>();

        foreach (var like in likes)
        {
            if (like.AuthorContent.Item is not Track track)
                continue;

            var audioUrl = await _audioUrlResolver.ResolveAsync(track, cancellationToken);
            //var imageUrl = track.ImageItemId is Guid imageItemId
            //    ? $"/api/storage/images/{imageItemId}"
            //    : null;
            var authorNames = like.AuthorContent.Authors
                .Select(x => x.Author.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            result.Add(new LikedTrackResponse(
                track.Id,
                track.ExternalContentId,
                track.Name,
                authorNames.Any() ? string.Join(", ", authorNames) : "Unknown Author",
                track.Album?.Name ?? "Unknown Album",
                like.LikedAt.ToString("dd.MM.yyyy"),
                track.DurationSeconds,
                track.ImageItem?.ImageList,
                audioUrl,
                like.Order));
        }

        return LikedTracksResult.Success(result);
    }

    public async Task<ChangeLikedTracksOrderResult> ChangeOrderAsync(
        Guid userId,
        int oldOrder,
        int newOrder,
        CancellationToken cancellationToken = default)
    {
        if (oldOrder <= 0 || newOrder <= 0)
        {
            return ChangeLikedTracksOrderResult.Failure(
                "OldOrder and NewOrder must be positive integers.");
        }

        if (oldOrder == newOrder)
        {
            return ChangeLikedTracksOrderResult.Success(oldOrder, newOrder);
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        var trackLikes = GetTrackLikesQuery(userId);
        var tracksCount = await trackLikes.CountAsync(cancellationToken);

        if (newOrder > tracksCount)
        {
            return ChangeLikedTracksOrderResult.Failure(
                "NewOrder is outside the liked tracks list.");
        }

        var movedLike = await trackLikes
            .FirstOrDefaultAsync(x => x.Order == oldOrder, cancellationToken);

        if (movedLike is null)
        {
            return ChangeLikedTracksOrderResult.Failure(
                "The liked track at OldOrder was not found.");
        }

        var affectedLikes = oldOrder < newOrder
            ? await trackLikes
                .Where(x => x.Order > oldOrder && x.Order <= newOrder)
                .ToListAsync(cancellationToken)
            : await trackLikes
                .Where(x => x.Order >= newOrder && x.Order < oldOrder)
                .ToListAsync(cancellationToken);

        foreach (var affectedLike in affectedLikes)
        {
            affectedLike.Order += oldOrder < newOrder ? -1 : 1;
        }

        movedLike.Order = newOrder;

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ChangeLikedTracksOrderResult.Success(oldOrder, newOrder);
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
                    .ThenInclude(aca => aca.Author)
            .Include(x => x.Album)
            .Include(x => x.ImageItem)
            .Include(x => x.AudioItem)
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

    private IQueryable<Like> GetTrackLikesQuery(Guid userId) => _context.Likes
        .Where(x => x.ApplicationUserId == userId)
        .Where(x => x.AuthorContent.Item is Track);

    private async Task<int> GetLastTrackOrderAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await GetTrackLikesQuery(userId)
            .Select(x => (int?)x.Order)
            .MaxAsync(cancellationToken) ?? 0;
    }

    public async Task<TrackPageResult> GetTrackPage(string trackId, CancellationToken cancellationToken = default)
    {
        var track = await GetOrCreateTrackAsync(trackId, cancellationToken);

        if(track is null)
        {
            return TrackPageResult.Failure("Track not found");
        }

        var authorsNames = track.AuthorContent?.Authors
            .Select(a => a.Author.Name)
            .ToList() ?? [];

        var authorsIds = track.AuthorContent?.Authors
            .Select(a => a.Author.Id.ToString())
            .ToList() ?? [];

        var authorIds = track.AuthorContent?.Authors.Select(a => a.AuthorId).ToList() ?? [];


        if (track.Provider == AudioProvider.Jamendo && track.ExternalContentId != null)
        {
            var recommendations = await GetJamendoRecommendations(track.ExternalContentId, cancellationToken);
            var popularTracksByAuthor = await GetJamendoPopularTracksByAuthor(track.ExternalContentId, cancellationToken);
            var albumsByAuthor = await GetJamendoAlbumsByAuthor(track.ExternalContentId, cancellationToken);

            var audioUrl = await _audioUrlResolver.ResolveAsync(track, cancellationToken);

            var response = new TrackPageResponse(
                Name: track.Name,
                Description: track.Description,
                AuthorsNames: authorsNames,
                AuthorsIds: authorsIds,
                AlbumName: track.Album?.Name ?? "Unknown Album",
                ImageUrl: track.ImageItem?.ImageList,
                AudioUrl: audioUrl,
                PlaysNumber: track.PlaysNumber,
                CreatedAt: track.CreatedAt,
                Recomendation: recommendations,
                PopularTracksByAuthor: popularTracksByAuthor,
                AlbumsByAuthor: albumsByAuthor
            );

            return TrackPageResult.Success(response);
        }

        var localRecommendations = await _context.Tracks
            .Where(t => t.DeletedAt == null && !t.IsDraft && t.Id != track.Id)
            .OrderBy(t => Guid.NewGuid())
            .Take(5)
            .Select(t => new TrackResponse(
                t.Id.ToString(), t.Name, t.Description, t.DurationSeconds,
                t.AlbumId.ToString(), null, t.GenreId, t.PlaysNumber,
                t.IsAdult, t.IsDraft, t.AudioItemId, t.ImageItemId,
                new List<string>(), t.CreatedAt,
                null,
                t.AuthorContent != null ? t.AuthorContent.Authors.Select(a => a.AuthorId.ToString()).ToList() : null
            ))
            .ToListAsync(cancellationToken);

        var localPopularTracksByAuthor = await _context.Tracks
            .Where(t => t.DeletedAt == null && 
                   !t.IsDraft && 
                   t.Id != track.Id &&
                   t.AuthorContent != null &&
                   t.AuthorContent.Authors.Any(a => authorIds.Contains(a.AuthorId)))
            .OrderByDescending(t => t.PlaysNumber)
            .Take(5)
            .Select(t => new TrackResponse(
                t.Id.ToString(), t.Name, t.Description, t.DurationSeconds,
                t.AlbumId.ToString(), null, t.GenreId, t.PlaysNumber,
                t.IsAdult, t.IsDraft, t.AudioItemId, t.ImageItemId,
                new List<string>(), t.CreatedAt,
                null,
                t.AuthorContent != null ? t.AuthorContent.Authors.Select(a => a.AuthorId.ToString()).ToList() : null
            ))
            .ToListAsync(cancellationToken);

        var localAlbumsByAuthor = await _context.Albums
            .Where(a => a.DeletedAt == null && 
                   !a.IsDraft &&
                   a.AuthorContent != null &&
                   a.AuthorContent.Authors.Any(ac => authorIds.Contains(ac.AuthorId)))
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new AlbumResponse(
                a.Id.ToString(),
                a.Name,
                a.Description,
                a.DurationSeconds,
                a.ImageItemId,
                a.IsDraft,
                a.GenreId,
                a.CreatedAt,
                a.ImageItem!.ImageList
            ))
            .ToListAsync(cancellationToken);

        var audioUrl2 = await _audioUrlResolver.ResolveAsync(track, cancellationToken);

        var responseLocal = new TrackPageResponse(
            Name: track.Name,
            Description: track.Description,
            AuthorsNames: authorsNames,
            AuthorsIds: authorsIds,
            AlbumName: track.Album?.Name ?? "Unknown Album",
            ImageUrl: track.ImageItem?.ImageList,
            AudioUrl: audioUrl2,
            PlaysNumber: track.PlaysNumber,
            CreatedAt: track.CreatedAt,
            Recomendation: localRecommendations,
            PopularTracksByAuthor: localPopularTracksByAuthor,
            AlbumsByAuthor: localAlbumsByAuthor
        );

        return TrackPageResult.Success(responseLocal);
    }

    private async Task<IReadOnlyCollection<TrackResponse>> GetJamendoRecommendations(string trackId, CancellationToken cancellationToken)
    {

        try
        {
            var track = await _jamendoService.GetTrackAsync(trackId, cancellationToken);
            if (track is null) return null;
            var authorId = track!.ArtistId;
            var recommendations = new List<TrackResponse>();
            var jamendoTracks = await _jamendoService.GetTracksByAuthorAsync(
                authorId,
                maxPerPage: 5,
                page: 1,
                cancellationToken: cancellationToken);

            if (jamendoTracks != null && jamendoTracks.Tracks.Count > 0)
            {
                recommendations = jamendoTracks.Tracks
                    .Select(jt => new TrackResponse(
                        jt.Id, jt.Name ?? "Unknown", null, jt.DurationSeconds,
                        jt.AlbumId, null, null, 0,
                        jt.IsExplicit, false, null, null,
                        new List<string>(), DateTime.UtcNow,
                        jt.AudioUrl,
                        null
                    ))
                    .Take(5)
                    .ToList();
            }

            return recommendations;
        }
        catch
        {
            return [];
        }
    }

    private async Task<IReadOnlyCollection<TrackResponse>> GetJamendoPopularTracksByAuthor(string trackId, CancellationToken cancellationToken)
    {
        var track = await _fromJamendoToLocalService.GetOrCreateJamendoTrackAsync(trackId, cancellationToken);
        if (track?.AuthorContent?.Authors.Count == 0)
            return [];

        var authorExternalId = track?.AuthorContent?.Authors.FirstOrDefault()?.Author.ExternalAuthorId;
        if (string.IsNullOrWhiteSpace(authorExternalId))
            return [];


        var jamendoTracksResult = await _jamendoService.GetTracksByAuthorAsync(authorExternalId, 5, 1, cancellationToken);

        if (jamendoTracksResult?.Tracks == null || jamendoTracksResult.Tracks.Count == 0)
            return [];
        

        return jamendoTracksResult.Tracks
            .Select(jt => new TrackResponse(
                jt.Id, jt.Name ?? "Unknown", null, jt.DurationSeconds,
                jt.AlbumId, null, null, 0,
                jt.IsExplicit, false, null, null,
                new List<string>(), DateTime.UtcNow,
                jt.AudioUrl,
                null
            ))
            .Take(5)
            .ToList();
    }

    private async Task<IReadOnlyCollection<AlbumResponse>> GetJamendoAlbumsByAuthor(string trackId, CancellationToken cancellationToken)
    {
        var track = await _jamendoService.GetTrackAsync(trackId, cancellationToken);
        if (track is null) return null;

        var authorId = track.ArtistId;
        var albums = (await _jamendoService.GetAlbumsByAuthorAsync(authorId, 5, 1, cancellationToken))?.Albums;
        if (albums is null) return null;
        return albums.Select(ja => new AlbumResponse(
            ja.Id, ja.Name, null, 0, null, false, null, ja.ReleaseDate, ja.ImageUrl)
            ).Take(5).ToList();
    }
}
