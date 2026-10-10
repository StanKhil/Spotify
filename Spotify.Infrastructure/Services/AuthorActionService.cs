using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Author;
using Spotify.Application.DTOs.Search;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AuthorActionService : IAuthorActionService
{
    private readonly ApplicationContext _context;
    private const int RecommendedTracksLimit = 10;
    private const int RecommendedAuthorsLimit = 5;
    private readonly IFromJamendoToLocalService _fromJamendoToLocalService;
    private readonly IJamendoService _jamendoService;

    public AuthorActionService(ApplicationContext context,
        IFromJamendoToLocalService fromJamendoToLocalService, IJamendoService jamendoService)
    {
        _context = context;
        _fromJamendoToLocalService = fromJamendoToLocalService;
        _jamendoService = jamendoService;
    }

    public async Task<AuthorActionResponse?> SubscribeAsync(
        string authorId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (_fromJamendoToLocalService.IsJamendoId(authorId.ToString()))
        {
            var jamendoAuthor = await _fromJamendoToLocalService.GetOrCreateJamendoAuthorAsync(
                authorId.ToString(),
                cancellationToken);

            if (jamendoAuthor is null)
                return null;

            authorId = jamendoAuthor.Id.ToString();
        }

        var author = await GetAuthorAsync(
            Guid.Parse(authorId),
            cancellationToken);

        if (author is null)
            return null;

        //if (author.User != null && author.User.Id != userId)
        //{
        //    return new AuthorActionResponse(
        //        author.Id,
        //        await GetSubscriptionsCountAsync(
        //            author.Id,
        //            cancellationToken),
        //        false);
        //}

        var alreadySubscribed =
            await _context.AuthorSubscriptions
                .AnyAsync(
                    x => x.ApplicationUserId == userId &&
                         x.AuthorId == Guid.Parse(authorId),
                    cancellationToken);

        if (!alreadySubscribed)
        {
            _context.AuthorSubscriptions.Add(
                new AuthorSubscription
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = userId,
                    AuthorId = Guid.Parse(authorId),
                    CreatedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        var subscriptionsCount =
            await GetSubscriptionsCountAsync(
                author.Id,
                cancellationToken);

        return new AuthorActionResponse(
            author.Id,
            subscriptionsCount,
            true);
    }

    public async Task<AuthorActionResponse?> UnsubscribeAsync(
        string authorId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var author = await GetAuthorAsync(
            Guid.Parse(authorId),
            cancellationToken);

        if (author is null)
            return null;

        var subscription =
            await _context.AuthorSubscriptions
                .FirstOrDefaultAsync(
                    x => x.ApplicationUserId == userId &&
                         x.AuthorId == Guid.Parse(authorId),
                    cancellationToken);

        if (subscription is not null)
        {
            _context.AuthorSubscriptions.Remove(subscription);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        var subscriptionsCount =
            await GetSubscriptionsCountAsync(
                author.Id,
                cancellationToken);

        return new AuthorActionResponse(
            author.Id,
            subscriptionsCount,
            false);
    }

    public async Task<SubscribedAuthorsResult> GetSubscribed(
    int maxPerPage,
    int page,
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        if (maxPerPage <= 0 || page <= 0)
        {
            return SubscribedAuthorsResult.Failure(
                "Invalid pagination parameters.");
        }

        var subscribedAuthorsQuery = _context.AuthorSubscriptions
            .Where(x => x.ApplicationUserId == userId);

        var authors = await subscribedAuthorsQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .Select(x => new AuthorResponse(
                x.AuthorId,
                x.Author.Name,
                x.Author.MonthList,
                x.Author.Bio,
                x.Author.BioImageItemId,
                x.Author.AuthoredContent.Count))
            .ToListAsync(cancellationToken);

        return SubscribedAuthorsResult.Success(
            new SubscribedAuthorsResponse(authors));
    }

    private async Task<Author?> GetAuthorAsync(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        var localAuthor = await _context.Authors
            .Where(x => x.Id == authorId)
            .FirstOrDefaultAsync(cancellationToken);

        if (localAuthor == null)
        {
            return await _context.Authors
                .Where(a => a.ExternalAuthorId == authorId.ToString())
                .FirstOrDefaultAsync(cancellationToken);
        }

        return localAuthor;
    }

    private async Task<int> GetSubscriptionsCountAsync(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        return await _context.AuthorSubscriptions
            .CountAsync(
                x => x.AuthorId == authorId,
                cancellationToken);
    }

    public async Task<AuthorPageResult> GetAuthorPageAsync(
        int maxPerPage,
        int page,
        string authorId,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || maxPerPage < 1)
        {
            return AuthorPageResult.Failure("Page and MaxPerPage must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(authorId))
        {
            return AuthorPageResult.Failure("Author ID cannot be empty.");
        }

        Author? author = null;

        if (Guid.TryParse(authorId, out var parsedGuid))
        {
            author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == parsedGuid, cancellationToken);
        }
        else if (_fromJamendoToLocalService.IsJamendoId(authorId))
        {
            author = await _fromJamendoToLocalService.GetOrCreateJamendoAuthorAsync(
                authorId, cancellationToken);
        }

        if (author is null)
        {
            return AuthorPageResult.Failure($"Author with ID '{authorId}' was not found.");
        }

        var internalAuthorId = author.Id;
        var tracksQuery = _context.Tracks
            .AsNoTracking()
            .Where(t => t.DeletedAt == null &&
                        t.AuthorContent != null &&
                        t.AuthorContent.Authors.Any(aca => aca.AuthorId == internalAuthorId));

        var totalTracks = await tracksQuery.CountAsync(cancellationToken);

        List<SearchTrackResponse> tracksItems;
        int totalTrackPages;

        if (totalTracks > 0)
        {
            totalTrackPages = (int)Math.Ceiling((double)totalTracks / maxPerPage);

            tracksItems = await tracksQuery
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(t => new SearchTrackResponse(
                    t.Id.ToString(),
                    t.Name,
                    t.Description,
                    t.DurationSeconds,
                    t.Provider.ToString(),
                    t.AlbumId != null ? t.AlbumId.ToString() : null,
                    t.Album != null ? t.Album.Name : null,
                    author.Name,
                    GetImageUrl(t.ImageItemId),
                    GetAudioUrl(t.Id)
                ))
                .ToListAsync(cancellationToken);
        }
        else if (!string.IsNullOrEmpty(author.ExternalAuthorId))
        {
            var jamendoTracksDto = await _jamendoService.GetTracksByAuthorAsync(
                author.ExternalAuthorId, maxPerPage, page, cancellationToken);

            var rawTracks = jamendoTracksDto?.Tracks ?? [];
            var totalJamendoTracks = rawTracks.Count;

            totalTracks = totalJamendoTracks;
            totalTrackPages = (int)Math.Ceiling((double)totalJamendoTracks / maxPerPage);
            tracksItems = rawTracks
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(t => new SearchTrackResponse(
                    t.Id,
                    t.Name,
                    null,
                    t.DurationSeconds,
                    "Jamendo",
                    t.AlbumId,
                    t.AlbumName,
                    author.Name,
                    t.ImageUrl,
                    t.AudioUrl))
                .ToList();
        }
        else
        {
            tracksItems = [];
            totalTrackPages = 0;
        }

        var tracksPage = new SearchPage<SearchTrackResponse>(tracksItems, totalTracks, page, totalTrackPages);

        var albumsQuery = _context.Albums
            .AsNoTracking()
            .Where(a => a.DeletedAt == null &&
                        a.AuthorContent != null &&
                        a.AuthorContent.Authors.Any(aca => aca.AuthorId == internalAuthorId));

        var totalAlbums = await albumsQuery.CountAsync(cancellationToken);

        List<SearchAlbumResponse> albumsItems;
        int totalAlbumPages;

        if (totalAlbums > 0)
        {
            totalAlbumPages = (int)Math.Ceiling((double)totalAlbums / maxPerPage);

            albumsItems = await albumsQuery
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(a => new SearchAlbumResponse(
                    a.Id.ToString(),
                    a.Name,
                    a.Description,
                    a.DurationSeconds,
                    a.Provider.ToString(),
                    author.Name,
                    GetImageUrl(a.ImageItemId),
                    a.Tracks.Count(t => t.DeletedAt == null),
                    a.CreatedAt
                ))
                .ToListAsync(cancellationToken);
        }
        else if (!string.IsNullOrEmpty(author.ExternalAuthorId))
        {
            var jamendoAlbumsDto = await _jamendoService.GetAlbumsByAuthorAsync(
                author.ExternalAuthorId, maxPerPage, page, cancellationToken);

            var rawAlbums = jamendoAlbumsDto?.Albums ?? [];
            var totalJamendoAlbums = rawAlbums.Count;

            totalAlbums = totalJamendoAlbums;
            totalAlbumPages = (int)Math.Ceiling((double)totalJamendoAlbums / maxPerPage);
            albumsItems = rawAlbums
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .Select(a => new SearchAlbumResponse(
                    a.Id,
                    a.Name,
                    null,
                    0,
                    "Jamendo",
                    author.Name,
                    a.ImageUrl,
                    a.TracksCount,
                    a.ReleaseDate
                ))
                .ToList();
        }
        else
        {
            albumsItems = [];
            totalAlbumPages = 0;
        }

        var albumsPage = new SearchPage<SearchAlbumResponse>(albumsItems, totalAlbums, page, totalAlbumPages);

        var interests = await _context.Tracks
            .AsNoTracking()
            .Where(t => t.DeletedAt == null)
            .OrderBy(t => EF.Functions.Random())
            .Take(RecommendedTracksLimit)
            .Select(t => new SearchTrackResponse(
                t.Id.ToString(),
                t.Name,
                t.Description,
                t.DurationSeconds,
                t.Provider.ToString(),
                t.AlbumId != null ? t.AlbumId.ToString() : null,
                t.Album != null ? t.Album.Name : null,
                t.AuthorContent != null
                    ? t.AuthorContent.Authors.Select(aca => aca.Author.Name).FirstOrDefault()
                    : null,
                GetImageUrl(t.ImageItemId),
                GetAudioUrl(t.Id)
            ))
            .ToListAsync(cancellationToken);
        var artists = await _context.Authors
            .AsNoTracking()
            .Where(a => a.Id != internalAuthorId)
            .OrderBy(a => EF.Functions.Random())
            .Take(RecommendedAuthorsLimit)
            .Select(a => new SearchAuthorResponse(
                a.Id.ToString(),
                a.Name,
                "LocalStorage",
                a.Bio,
                GetImageUrl(a.BioImageItemId),
                a.AuthoredContent.Count,
                null
            ))
            .ToListAsync(cancellationToken);

        var authorPageResponse = new AuthorPageResponse(
            author.Id,
            author.Name,
            author.Bio,
            GetImageUrl(author.BioImageItemId),
            tracksPage,
            albumsPage,
            interests,
            artists
        );

        return AuthorPageResult.Success(authorPageResponse);
    }

    private static string? GetImageUrl(Guid? imageItemId) =>
        imageItemId is Guid id && id != Guid.Empty
            ? $"/images/{id}"
            : null;

    private static string GetAudioUrl(Guid trackId) =>
        $"/api/playback/tracks/{trackId}";
}