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
    private const int JamendoCandidateLimit = 200;
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
                x.Author.BioImageItem!.ImageList,
                x.Author.AuthoredContent.Count))
            .ToListAsync(cancellationToken);

        return SubscribedAuthorsResult.Success(authors);
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
        string authorId,
        int tracksMaxPerPage,
        int tracksPage,
        int albumsMaxPerPage,
        int albumsPage,
        int interestsMaxPerPage,
        int interestsPage,
        int artistsMaxPerPage,
        int artistsPage,
        CancellationToken cancellationToken = default)
    {
        if (tracksPage < 1 || tracksMaxPerPage < 1 ||
            albumsPage < 1 || albumsMaxPerPage < 1 ||
            interestsPage < 1 || interestsMaxPerPage < 1 ||
            artistsPage < 1 || artistsMaxPerPage < 1)
        {
            return AuthorPageResult.Failure("All Page and MaxPerPage values must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(authorId))
        {
            return AuthorPageResult.Failure("Author ID cannot be empty.");
        }

        Author? author = null;

        if (Guid.TryParse(authorId, out var parsedGuid))
        {
            author = await _context.Authors
                .Include(a => a.BioImageItem)
                .Include(a => a.Followers)
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

        List<SearchTrackResponse> tracksItems;
        int totalTracks;
        int totalTrackPages;

        if (!string.IsNullOrEmpty(author.ExternalAuthorId))
        {
            var jamendoTracksDto = await _jamendoService.GetTracksByAuthorAsync(
                author.ExternalAuthorId, JamendoCandidateLimit, 1, cancellationToken);

            var rawTracks = jamendoTracksDto?.Tracks ?? [];
            totalTracks = rawTracks.Count;
            totalTrackPages = (int)Math.Ceiling((double)totalTracks / tracksMaxPerPage);

            tracksItems = rawTracks
                .Skip((tracksPage - 1) * tracksMaxPerPage)
                .Take(tracksMaxPerPage)
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
            var tracksQuery = _context.Tracks
                .AsNoTracking()
                .Where(t => t.DeletedAt == null &&
                            t.AuthorContent != null &&
                            t.AuthorContent.Authors.Any(aca => aca.AuthorId == internalAuthorId));

            totalTracks = await tracksQuery.CountAsync(cancellationToken);
            totalTrackPages = (int)Math.Ceiling((double)totalTracks / tracksMaxPerPage);

            tracksItems = (await tracksQuery
                .OrderByDescending(t => t.CreatedAt)
                .Skip((tracksPage - 1) * tracksMaxPerPage)
                .Take(tracksMaxPerPage)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .Select(t => new SearchTrackResponse(
                    t.Id.ToString(),
                    t.Name,
                    t.Description,
                    t.DurationSeconds,
                    t.Provider.ToString(),
                    t.AlbumId != null ? t.AlbumId.ToString() : null,
                    t.Album != null ? t.Album.Name : null,
                    author.Name,
                    GetImageUrl(t.ImageItemId, t.ImageItem?.ImageList),
                    GetAudioUrl(t.Id)
                ))
                .ToList();
        }

        var tracksPageResult = new SearchPage<SearchTrackResponse>(
            tracksItems, totalTracks, tracksPage, totalTrackPages);

        List<SearchAlbumResponse> albumsItems;
        int totalAlbums;
        int totalAlbumPages;

        if (!string.IsNullOrEmpty(author.ExternalAuthorId))
        {
            var jamendoAlbumsDto = await _jamendoService.GetAlbumsByAuthorAsync(
                author.ExternalAuthorId, JamendoCandidateLimit, 1, cancellationToken);

            var rawAlbums = jamendoAlbumsDto?.Albums ?? [];
            totalAlbums = rawAlbums.Count;
            totalAlbumPages = (int)Math.Ceiling((double)totalAlbums / albumsMaxPerPage);

            albumsItems = rawAlbums
                .Skip((albumsPage - 1) * albumsMaxPerPage)
                .Take(albumsMaxPerPage)
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
            var albumsQuery = _context.Albums
                .Include(ii => ii.ImageItem)
                .AsNoTracking()
                .Where(a => a.DeletedAt == null &&
                            a.AuthorContent != null &&
                            a.AuthorContent.Authors.Any(aca => aca.AuthorId == internalAuthorId));

            totalAlbums = await albumsQuery.CountAsync(cancellationToken);
            totalAlbumPages = (int)Math.Ceiling((double)totalAlbums / albumsMaxPerPage);

            albumsItems = (await albumsQuery
                .OrderByDescending(a => a.CreatedAt)
                .Skip((albumsPage - 1) * albumsMaxPerPage)
                .Take(albumsMaxPerPage)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .Select(a => new SearchAlbumResponse(
                    a.Id.ToString(),
                    a.Name,
                    a.Description,
                    a.DurationSeconds,
                    a.Provider.ToString(),
                    author.Name,
                    GetImageUrl(a.ImageItemId, a.ImageItem?.ImageList),
                    a.Tracks.Count(t => t.DeletedAt == null),
                    a.CreatedAt
                ))
                .ToList();
        }

        var albumsPageResult = new SearchPage<SearchAlbumResponse>(
            albumsItems, totalAlbums, albumsPage, totalAlbumPages);

        var interestsQuery = _context.Tracks
            .Include(ii => ii.ImageItem)
            .AsNoTracking()
            .Where(t => t.DeletedAt == null);

        var totalInterests = await interestsQuery.CountAsync(cancellationToken);
        var totalInterestPages = (int)Math.Ceiling((double)totalInterests / interestsMaxPerPage);

        var interestsItems = (await interestsQuery
            .OrderBy(t => EF.Functions.Random())
            .Skip((interestsPage - 1) * interestsMaxPerPage)
            .Take(interestsMaxPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken))
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
                GetImageUrl(t.ImageItemId, t.ImageItem?.ImageList),
                GetAudioUrl(t.Id)
            ))
            .ToList();

        var interestsPageResult = new SearchPage<SearchTrackResponse>(
            interestsItems, totalInterests, interestsPage, totalInterestPages);

        var artistsQuery = _context.Authors
            .Include(ii => ii.BioImageItem)
            .AsNoTracking()
            .Where(a => a.Id != internalAuthorId);

        var totalArtists = await artistsQuery.CountAsync(cancellationToken);
        var totalArtistPages = (int)Math.Ceiling((double)totalArtists / artistsMaxPerPage);

        var artistsItems = (await artistsQuery
            .OrderBy(a => EF.Functions.Random())
            .Skip((artistsPage - 1) * artistsMaxPerPage)
            .Take(artistsMaxPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .Select(a => new SearchAuthorResponse(
                a.Id.ToString(),
                a.Name,
                "LocalStorage",
                a.Bio,
                GetImageUrl(a.BioImageItemId, a.BioImageItem?.ImageList),
                a.AuthoredContent.Count,
                null
            ))
            .ToList();

        var artistsPageResult = new SearchPage<SearchAuthorResponse>(
            artistsItems, totalArtists, artistsPage, totalArtistPages);

        var authorPageResponse = new AuthorPageResponse(
            Id: author.Id,
            Name: author.Name,
            Description: author.Bio,
            ImageUrl: GetImageUrl(author.BioImageItemId, author.BioImageItem?.ImageList),
            Listeners: author.Followers.Count,
            Tracks: tracksPageResult,
            Music: albumsPageResult,
            Interests: interestsPageResult,
            Artists: artistsPageResult
        );

       return AuthorPageResult.Successed(authorPageResponse);
    }

    private static string? GetImageUrl(Guid? imageItemId, string? imageItemUrl)
    {
        if(imageItemId is null && string.IsNullOrEmpty(imageItemUrl))
        {
            return null;
        }
        if (imageItemUrl!.StartsWith("/"))
        {
            return imageItemId.ToString();
        }
        return imageItemUrl;
    }

    private static string GetAudioUrl(Guid trackId) =>
        $"/api/playback/tracks/{trackId}";
}