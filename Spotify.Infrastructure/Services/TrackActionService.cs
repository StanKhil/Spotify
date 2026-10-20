using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
using Spotify.Application.DTOs.Search;
using Spotify.Application.DTOs.Track;
using Spotify.Application.DTOs.Author;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class TrackActionService : ITrackActionService
{
    private readonly byte _maxHistoryItems = 5;
    private const int JamendoCandidateLimit = 200;
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
                ImageUrl: track.ImageItem?.ImageList,
                AudioUrl: audioUrl,
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

            var imageUrl = track.ImageItem?.ImageList;
            if (imageUrl?[0] == '/') imageUrl = track.ImageItemId.ToString();

            result.Add(new ListeningHistoryResponse(
                item.Id,
                track.Id,
                track.Name,
                artistName,
                track.Album?.Name ?? "Unknown Album",
                track.DurationSeconds.ToString(),
                audioUrl,
                imageUrl
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

    public async Task<TrackPageResult> GetTrackPageAsync(
    string trackId,
    int recommendationsMaxPerPage,
    int recommendationsPage,
    int popularMaxPerPage,
    int popularPage,
    int albumsMaxPerPage,
    int albumsPage,
    CancellationToken cancellationToken = default)
    {
        if (recommendationsPage < 1 || recommendationsMaxPerPage < 1 ||
            popularPage < 1 || popularMaxPerPage < 1 ||
            albumsPage < 1 || albumsMaxPerPage < 1)
        {
            return TrackPageResult.Failure("All Page and MaxPerPage values must be greater than 0.");
        }

        var track = await GetOrCreateTrackAsync(trackId, cancellationToken);

        if (track is null)
        {
            return TrackPageResult.Failure("Track not found");
        }

        var authorIds = track.AuthorContent?.Authors.Select(a => a.AuthorId).ToList() ?? [];
        var authors = track.AuthorContent?.Authors.Select(a => a.Author).ToList() ?? [];

        var authorsResponse = authors.Select(a => new AuthorResponse(
            a.Id,
            a.Name,
            a.MonthList,
            a.Bio,
            GetImageUrl(a.BioImageItemId) ?? a.BioImageItem?.ImageList,
            a.AuthoredContent?.Count ?? 0
        )).ToList();

        if (track.Provider == AudioProvider.Jamendo && track.ExternalContentId != null)
        {
            var recommendationsPageResult = await GetJamendoRecommendationsAsync(
                track.ExternalContentId, recommendationsMaxPerPage, recommendationsPage, cancellationToken);

            var popularTracksPageResult = await GetJamendoPopularTracksByAuthorAsync(
                track.ExternalContentId, popularMaxPerPage, popularPage, cancellationToken);

            var albumsPageResult = await GetJamendoAlbumsByAuthorAsync(
                track.ExternalContentId, albumsMaxPerPage, albumsPage, cancellationToken);

            var audioUrl = await _audioUrlResolver.ResolveAsync(track, cancellationToken);

            var response = new TrackPageResponse(
                Name: track.Name,
                Description: track.Description,
                Authors: authorsResponse,
                AlbumName: track.Album?.Name ?? "Unknown Album",
                AlbumId: track.AlbumId?.ToString(),
                ImageUrl: track.ImageItem != null && track.ImageItem.ImageList.StartsWith("/")
                    ? track.ImageItemId?.ToString()
                    : track.ImageItem!.ImageList,
                AudioUrl: audioUrl,
                Listeners: track.PlaysNumber,
                DurationSeconds: track.DurationSeconds,
                CreatedAt: track.CreatedAt,
                Recomendation: recommendationsPageResult,
                PopularTracksByAuthor: popularTracksPageResult,
                AlbumsByAuthor: albumsPageResult
            );

            return TrackPageResult.Success(response);
        }

        var recQuery = _context.Tracks
            .AsNoTracking()
            .Where(t => t.DeletedAt == null && !t.IsDraft && t.Id != track.Id);

        var totalRecs = await recQuery.CountAsync(cancellationToken);
        var totalRecPages = (int)Math.Ceiling((double)totalRecs / recommendationsMaxPerPage);

        var localRecommendations = await recQuery
            .OrderBy(t => EF.Functions.Random())
            .Skip((recommendationsPage - 1) * recommendationsMaxPerPage)
            .Take(recommendationsMaxPerPage)
            .Select(t => new TrackRecommendationResponse(
                t.Id.ToString(),
                t.Name,
                t.DurationSeconds,
                t.AlbumId != null ? t.AlbumId.ToString() : null,
                t.AuthorContent != null
                    ? t.AuthorContent.Authors.Select(aca => aca.Author.Name).FirstOrDefault()
                    : null,
                GetAudioUrl(t.Id),
                GetImageUrl(t.ImageItemId)
            ))
            .ToListAsync(cancellationToken);

        var localRecsPage = new SearchPage<TrackRecommendationResponse>(
            localRecommendations, totalRecs, recommendationsPage, totalRecPages);

        var popularQuery = _context.Tracks
            .AsNoTracking()
            .Where(t => t.DeletedAt == null &&
                        !t.IsDraft &&
                        t.Id != track.Id &&
                        t.AuthorContent != null &&
                        t.AuthorContent.Authors.Any(a => authorIds.Contains(a.AuthorId)));

        var totalPopular = await popularQuery.CountAsync(cancellationToken);
        var totalPopularPages = (int)Math.Ceiling((double)totalPopular / popularMaxPerPage);

        var localPopularTracks = await popularQuery
            .OrderByDescending(t => t.PlaysNumber)
            .Skip((popularPage - 1) * popularMaxPerPage)
            .Take(popularMaxPerPage)
            .Select(t => new TrackPopularResponse(
                t.Id.ToString(),
                t.Name,
                t.DurationSeconds,
                t.AlbumId != null ? t.AlbumId.ToString() : null,
                GetAudioUrl(t.Id),
                GetImageUrl(t.ImageItemId)
            ))
            .ToListAsync(cancellationToken);

        var localPopularPage = new SearchPage<TrackPopularResponse>(
            localPopularTracks, totalPopular, popularPage, totalPopularPages);

        var albumsQuery = _context.Albums
            .AsNoTracking()
            .Where(a => a.DeletedAt == null &&
                        !a.IsDraft &&
                        a.AuthorContent != null &&
                        a.AuthorContent.Authors.Any(ac => authorIds.Contains(ac.AuthorId)));

        var totalAlbums = await albumsQuery.CountAsync(cancellationToken);
        var totalAlbumPages = (int)Math.Ceiling((double)totalAlbums / albumsMaxPerPage);

        var localAlbums = await albumsQuery
            .OrderByDescending(a => a.CreatedAt)
            .Skip((albumsPage - 1) * albumsMaxPerPage)
            .Take(albumsMaxPerPage)
            .Select(a => new TrackAlbumResponse(
                a.Id.ToString(),
                a.Name,
                a.CreatedAt,
                GetImageUrl(a.ImageItemId)
            ))
            .ToListAsync(cancellationToken);

        var localAlbumsPage = new SearchPage<TrackAlbumResponse>(
            localAlbums, totalAlbums, albumsPage, totalAlbumPages);

        var audioUrlLocal = await _audioUrlResolver.ResolveAsync(track, cancellationToken);

        var responseLocal = new TrackPageResponse(
            Name: track.Name,
            Description: track.Description,
            Authors: authorsResponse,
            AlbumName: track.Album?.Name ?? "Unknown Album",
            AlbumId: track.AlbumId?.ToString(),
            ImageUrl: GetImageUrl(track.ImageItemId) ?? track.ImageItem?.ImageList,
            AudioUrl: audioUrlLocal,
            Listeners: track.PlaysNumber,
            DurationSeconds: track.DurationSeconds,
            CreatedAt: track.CreatedAt,
            Recomendation: localRecsPage,
            PopularTracksByAuthor: localPopularPage,
            AlbumsByAuthor: localAlbumsPage
        );

        return TrackPageResult.Success(responseLocal);
    }

    private async Task<SearchPage<TrackRecommendationResponse>> GetJamendoRecommendationsAsync(
    string trackId, int maxPerPage, int page, CancellationToken cancellationToken)
    {
        try
        {
            var track = await _jamendoService.GetTrackAsync(trackId, cancellationToken);
            if (track is null) return EmptySearchPage<TrackRecommendationResponse>(page);

            var authorId = track.ArtistId;
            var jamendoTracks = await _jamendoService.GetTracksByAuthorAsync(
                authorId, JamendoCandidateLimit, 1, cancellationToken);

            var rawTracks = jamendoTracks?.Tracks ?? [];
            var totalCount = rawTracks.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / maxPerPage);

            var items = rawTracks
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(jt => new TrackRecommendationResponse(
                    Id: jt.Id,
                    Name: jt.Name ?? "Unknown",
                    DurationSeconds: jt.DurationSeconds,
                    AlbumId: jt.AlbumId,
                    AuthorName: jt.ArtistName ?? track.ArtistName,
                    AudioUrl: jt.AudioUrl,
                    ImageUrl: jt.ImageUrl
                ))
                .ToList();

            return new SearchPage<TrackRecommendationResponse>(items, totalCount, page, totalPages);
        }
        catch
        {
            return EmptySearchPage<TrackRecommendationResponse>(page);
        }
    }

    private async Task<SearchPage<TrackPopularResponse>> GetJamendoPopularTracksByAuthorAsync(
        string trackId, int maxPerPage, int page, CancellationToken cancellationToken)
    {
        try
        {
            var track = await _fromJamendoToLocalService.GetOrCreateJamendoTrackAsync(trackId, cancellationToken);
            var authorExternalId = track?.AuthorContent?.Authors.FirstOrDefault()?.Author.ExternalAuthorId;

            if (string.IsNullOrWhiteSpace(authorExternalId))
                return EmptySearchPage<TrackPopularResponse>(page);

            var jamendoTracksResult = await _jamendoService.GetTracksByAuthorAsync(
                authorExternalId, JamendoCandidateLimit, 1, cancellationToken);

            var rawTracks = jamendoTracksResult?.Tracks ?? [];
            var totalCount = rawTracks.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / maxPerPage);

            var items = rawTracks
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(jt => new TrackPopularResponse(
                    Id: jt.Id,
                    Name: jt.Name ?? "Unknown",
                    DurationSeconds: jt.DurationSeconds,
                    AlbumId: jt.AlbumId,
                    AudioUrl: jt.AudioUrl,
                    ImageUrl: jt.ImageUrl
                ))
                .ToList();

            return new SearchPage<TrackPopularResponse>(items, totalCount, page, totalPages);
        }
        catch
        {
            return EmptySearchPage<TrackPopularResponse>(page);
        }
    }

    private async Task<SearchPage<TrackAlbumResponse>> GetJamendoAlbumsByAuthorAsync(
        string trackId, int maxPerPage, int page, CancellationToken cancellationToken)
    {
        try
        {
            var track = await _jamendoService.GetTrackAsync(trackId, cancellationToken);
            if (track is null) return EmptySearchPage<TrackAlbumResponse>(page);

            var authorId = track.ArtistId;
            var jamendoAlbums = await _jamendoService.GetAlbumsByAuthorAsync(
                authorId, JamendoCandidateLimit, 1, cancellationToken);

            var rawAlbums = jamendoAlbums?.Albums ?? [];
            var totalCount = rawAlbums.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / maxPerPage);

            var items = rawAlbums
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(ja => new TrackAlbumResponse(
                    Id: ja.Id,
                    Name: ja.Name,
                    ReleaseDate: ja.ReleaseDate,
                    ImageUrl: ja.ImageUrl
                ))
                .ToList();

            return new SearchPage<TrackAlbumResponse>(items, totalCount, page, totalPages);
        }
        catch
        {
            return EmptySearchPage<TrackAlbumResponse>(page);
        }
    }

    private static SearchPage<T> EmptySearchPage<T>(int page) => new([], 0, page, 0);

    private static string? GetImageUrl(Guid? imageItemId) =>
        imageItemId is Guid id && id != Guid.Empty
            ? $"/images/{id}"
            : null;

    private static string GetAudioUrl(Guid trackId) =>
        $"/api/playback/tracks/{trackId}";
}
