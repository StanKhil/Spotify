using Spotify.Application.DTOs.Album;
using Spotify.Application.DTOs.Author;
using Spotify.Application.DTOs.Jamendo;
using Spotify.Application.DTOs.Search;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;

namespace Spotify.Infrastructure.Services;

public sealed class SearchService : ISearchService
{
    // Jamendo limits a single query to 200 items and does not provide total-count metadata.
    private const int JamendoCandidateLimit = 200;

    private readonly ITrackService _trackService;
    private readonly IAlbumService _albumService;
    private readonly IAuthorService _authorService;
    private readonly IJamendoService _jamendoService;

    public SearchService(
        ITrackService trackService,
        IAlbumService albumService,
        IAuthorService authorService,
        IJamendoService jamendoService)
    {
        _trackService = trackService;
        _albumService = albumService;
        _authorService = authorService;
        _jamendoService = jamendoService;
    }

    public async Task<SearchPage<SearchTrackResponse>> SearchTracksAsync(
        string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken = default)
    {
        var localTask = _trackService.SearchTracksAsync(query, cancellationToken);
        var jamendoTask = _jamendoService.SearchTracksAsync(
            query, JamendoCandidateLimit, 1, cancellationToken);
        await Task.WhenAll(localTask, jamendoTask);

        var results = localTask.Result.Select(MapLocalTrack)
            .Concat(jamendoTask.Result.Select(MapJamendoTrack))
            .ToList();

        return CreatePage(results, maxPerPage, page);
    }

    public async Task<SearchPage<SearchAlbumResponse>> SearchAlbumsAsync(
        string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken = default)
    {
        var localTask = _albumService.SearchAlbumsAsync(query, cancellationToken);
        var jamendoTask = _jamendoService.SearchAlbumsAsync(
            query, JamendoCandidateLimit, 1, cancellationToken);
        await Task.WhenAll(localTask, jamendoTask);

        var results = localTask.Result.Select(MapLocalAlbum)
            .Concat(jamendoTask.Result.Select(MapJamendoAlbum))
            .ToList();

        return CreatePage(results, maxPerPage, page);
    }

    public async Task<SearchPage<SearchAuthorResponse>> SearchAuthorsAsync(
        string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken = default)
    {
        var localTask = _authorService.SearchAuthorsAsync(query, cancellationToken);
        var jamendoTask = _jamendoService.SearchAuthorsAsync(
            query, JamendoCandidateLimit, 1, cancellationToken);
        await Task.WhenAll(localTask, jamendoTask);

        var results = localTask.Result.Select(MapLocalAuthor)
            .Concat(jamendoTask.Result.Select(MapJamendoAuthor))
            .ToList();

        return CreatePage(results, maxPerPage, page);
    }

    private static SearchPage<T> CreatePage<T>(IReadOnlyCollection<T> results, int maxPerPage, int page)
    {
        var totalCount = results.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / maxPerPage);
        var items = results.Skip((page - 1) * maxPerPage).Take(maxPerPage).ToList();

        return new SearchPage<T>(items, totalCount, page, totalPages);
    }

    private static SearchTrackResponse MapLocalTrack(TrackResponse track) => new(
        track.Id.ToString(), track.Name, track.Description, track.DurationSeconds,
        "LocalStorage", track.AlbumId?.ToString(), null, null,
        GetImageUrl(track.ImageItemId), $"/api/playback/tracks/{track.Id}");

    private static SearchTrackResponse MapJamendoTrack(JamendoTrackDto track) => new(
        track.Id, track.Name, null, track.DurationSeconds, track.Provider,
        track.AlbumId, track.AlbumName, track.ArtistName, track.ImageUrl, track.AudioUrl);

    private static SearchAlbumResponse MapLocalAlbum(AlbumResponse album) => new(
        album.Id.ToString(), album.Name, album.Description, album.DurationSeconds,
        "LocalStorage", null, GetImageUrl(album.CoverImageId), 0, album.CreatedAt);

    private static SearchAlbumResponse MapJamendoAlbum(JamendoAlbumDto album) => new(
        album.Id, album.Name, null, 0, "Jamendo", album.ArtistName,
        album.ImageUrl, album.TracksCount, album.ReleaseDate);

    private static SearchAuthorResponse MapLocalAuthor(AuthorResponse author) => new(
        author.Id.ToString(), author.Name, "LocalStorage", author.Bio,
        GetImageUrl(author.BioImageItemId), author.ContentCount, null);

    private static SearchAuthorResponse MapJamendoAuthor(JamendoAuthorDto author) => new(
        author.Id, author.Name, "Jamendo", null, author.ImageUrl,
        0, author.WebsiteUrl);

    private static string? GetImageUrl(Guid? imageItemId) =>
        imageItemId is Guid id && id != Guid.Empty
            ? $"/api/storage/images/{id}"
            : null;
}
