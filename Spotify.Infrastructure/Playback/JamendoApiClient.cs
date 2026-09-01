using Microsoft.AspNetCore.WebUtilities;
using Spotify.Application.DTOs.Jamendo;
using System.Globalization;
using System.Text.Json;

namespace Spotify.Infrastructure.Playback;

public sealed class JamendoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JamendoOptions _options;

    public JamendoApiClient(
        HttpClient httpClient,
        JamendoOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<IReadOnlyCollection<JamendoTrackDto>> SearchTracksAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException(
                "Search query cannot be empty.",
                nameof(query));

        if (limit <= 0 || limit > 200)
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 200.");

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["search"] = query,
            ["audioformat"] = _options.AudioFormat,
            ["limit"] = limit.ToString()
        };

        var requestUri = QueryHelpers.AddQueryString(
            "tracks/",
            parameters);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty(
                "results",
                out var results) ||
            results.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var tracks = new List<JamendoTrackDto>();

        foreach (var item in results.EnumerateArray())
        {
            tracks.Add(MapTrack(item));
        }

        return tracks;
    }

    public async Task<JamendoTrackDto?> GetTrackAsync(
        string trackId,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        if (string.IsNullOrWhiteSpace(trackId))
            return null;

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["id"] = trackId,
            ["audioformat"] = _options.AudioFormat,
            ["limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString(
            "tracks/",
            parameters);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty(
                "results",
                out var results) ||
            results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            return null;
        }

        return MapTrack(results[0]);
    }

    public async Task<string?> GetTrackStreamUrlAsync(
        string trackId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(
            trackId,
            cancellationToken);

        return track?.AudioUrl;
    }

    public async Task<IReadOnlyCollection<JamendoAlbumDto>> SearchAlbumsAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException(
                "Search query cannot be empty.",
                nameof(query));

        if (limit <= 0 || limit > 200)
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 200.");

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["search"] = query,
            ["limit"] = limit.ToString()
        };

        var requestUri = QueryHelpers.AddQueryString(
            "albums/",
            parameters);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty(
                "results",
                out var results) ||
            results.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var albums = new List<JamendoAlbumDto>();

        foreach (var item in results.EnumerateArray())
        {
            albums.Add(
                MapAlbum(item));
        }

        return albums;
    }

    public async Task<JamendoAlbumTrackDto?> GetAlbumAsync(
        string albumId,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        if (string.IsNullOrWhiteSpace(albumId))
            return null;

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["id"] = albumId,
            ["limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString(
            "albums/tracks",
            parameters);

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty(
                "results",
                out var results) ||
            results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            return null;
        }

        return MapAlbumWithTracks(results[0]);
    }

    public async Task<JamendoAuthorDto?> GetAuthorAsync(
        string authorId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorId))
        {
            return null;
        }

        var results = await GetResultsAsync(
            "artists/",
            new Dictionary<string, string?>
            {
                ["id"] = authorId,
                ["limit"] = "1"
            },
            cancellationToken);

        return results.GetArrayLength() == 0 ? null : MapAuthor(results[0]);
    }

    public async Task<IReadOnlyCollection<JamendoAuthorDto>> SearchAuthorsAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search query cannot be empty.", nameof(query));
        }

        ValidateLimit(limit);
        var results = await GetResultsAsync(
            "artists/",
            new Dictionary<string, string?>
            {
                ["namesearch"] = query,
                ["limit"] = limit.ToString(CultureInfo.InvariantCulture)
            },
            cancellationToken);

        return results.EnumerateArray().Select(MapAuthor).ToArray();
    }

    public async Task<JamendoAuthorTracksDto?> GetTracksByAuthorAsync(
        string authorId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorId))
        {
            return null;
        }

        ValidateLimit(limit);
        var results = await GetResultsAsync(
            "artists/tracks/",
            new Dictionary<string, string?>
            {
                ["id"] = authorId,
                ["track_type"] = "single albumtrack",
                ["audioformat"] = _options.AudioFormat,
                ["limit"] = limit.ToString(CultureInfo.InvariantCulture)
            },
            cancellationToken);

        if (results.GetArrayLength() == 0)
        {
            return null;
        }

        var author = results[0];
        var tracks = author.TryGetProperty("tracks", out var tracksElement) &&
                     tracksElement.ValueKind == JsonValueKind.Array
            ? tracksElement.EnumerateArray().Select(track => MapAuthorTrack(track, author)).ToArray()
            : [];

        return new JamendoAuthorTracksDto(MapAuthor(author), tracks);
    }

    public async Task<JamendoAuthorAlbumsDto?> GetAlbumsByAuthorAsync(
        string authorId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorId))
        {
            return null;
        }

        ValidateLimit(limit);
        var results = await GetResultsAsync(
            "artists/albums/",
            new Dictionary<string, string?>
            {
                ["id"] = authorId,
                ["limit"] = limit.ToString(CultureInfo.InvariantCulture)
            },
            cancellationToken);

        if (results.GetArrayLength() == 0)
        {
            return null;
        }

        var author = results[0];
        var albums = author.TryGetProperty("albums", out var albumsElement) &&
                     albumsElement.ValueKind == JsonValueKind.Array
            ? albumsElement.EnumerateArray().Select(album => MapAuthorAlbum(album, author)).ToArray()
            : [];

        return new JamendoAuthorAlbumsDto(MapAuthor(author), albums);
    }

    private static JamendoTrackDto MapTrack(
        JsonElement track)
    {
        return new JamendoTrackDto(
            Id: GetString(track, "id"),
            Name: GetString(track, "name"),
            ArtistName: GetString(track, "artist_name"),
            ArtistId: GetString(track, "artist_id"),
            AlbumName: GetString(track, "album_name"),
            AlbumId: GetString(track, "album_id"),
            DurationSeconds: GetInt(track, "duration"),
            AudioUrl: GetString(track, "audio"),
            ImageUrl: GetString(track, "image"),
            IsExplicit: false,
            Provider: "Jamendo");
    }

    private static JamendoAuthorDto MapAuthor(JsonElement author) =>
        new(
            Id: GetString(author, "id"),
            Name: GetString(author, "name"),
            ImageUrl: GetString(author, "image"),
            WebsiteUrl: GetString(author, "website"),
            ShortUrl: GetString(author, "shorturl"),
            ShareUrl: GetString(author, "shareurl"),
            JoinedAt: ParseDate(author, "joindate"));

    private static JamendoTrackDto MapAuthorTrack(JsonElement track, JsonElement author) =>
        new(
            Id: GetString(track, "id"),
            Name: GetString(track, "name"),
            ArtistName: GetString(author, "name"),
            ArtistId: GetString(author, "id"),
            AlbumName: GetString(track, "album_name"),
            AlbumId: GetString(track, "album_id"),
            DurationSeconds: GetInt(track, "duration"),
            AudioUrl: GetString(track, "audio"),
            ImageUrl: GetString(track, "image"),
            IsExplicit: false,
            Provider: "Jamendo");

    private static JamendoAlbumDto MapAuthorAlbum(JsonElement album, JsonElement author) =>
        new(
            Id: GetString(album, "id"),
            Name: GetString(album, "name"),
            ArtistId: GetString(author, "id"),
            ArtistName: GetString(author, "name"),
            ImageUrl: GetString(album, "image"),
            TracksCount: GetInt(album, "tracks_count"),
            ReleaseDate: ParseDate(album, "releasedate"));

    private static JamendoAlbumDto MapAlbum(
        JsonElement album)
    {
        return new JamendoAlbumDto(
            Id: GetString(album, "id"),
            Name: GetString(album, "name"),
            ArtistName: GetString(album, "artist_name"),
            ArtistId: GetString(album, "artist_id"),
            ImageUrl: GetString(album, "image"),
            TracksCount: GetInt(album, "tracks_count"),
            ReleaseDate: ParseDate(
                album,
                "releasedate"));
    }

    private static JamendoAlbumTrackDto MapAlbumWithTracks(
        JsonElement album)
    {
        var tracks = new List<JamendoTrackDto>();

        if (album.TryGetProperty(
                "tracks",
                out var tracksElement) &&
            tracksElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var track in tracksElement.EnumerateArray())
            {
                tracks.Add(
                    MapAlbumTrack(
                        track,
                        album));
            }
        }

        return new JamendoAlbumTrackDto(
            Id: GetString(album, "id"),
            Name: GetString(album, "name"),
            ArtistId: GetString(album, "artist_id"),
            ArtistName: GetString(album, "artist_name"),
            ImageUrl: GetString(album, "image"),
            TracksCount: tracks.Count,
            ReleaseDate: ParseDate(
                album,
                "releasedate"),
            Tracks: tracks);
    }

    private static JamendoTrackDto MapAlbumTrack(
        JsonElement track,
        JsonElement album)
    {
        return new JamendoTrackDto(
            Id: GetString(track, "id"),
            Name: GetString(track, "name"),
            ArtistId: GetString(album, "artist_id"),
            ArtistName: GetString(album, "artist_name"),

            AlbumId: GetString(album, "id"),
            AlbumName: GetString(album, "name"),

            DurationSeconds: GetInt(
                track,
                "duration"),

            AudioUrl: GetString(
                track,
                "audio"),

            ImageUrl: GetString(
                album,
                "image"),

            IsExplicit: false,
            Provider: "Jamendo");
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        return element.TryGetProperty(
                propertyName,
                out var property) &&
            property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? string.Empty
            : string.Empty;
    }

    private static int GetInt(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return 0;
        }

        if (property.ValueKind == JsonValueKind.String &&
            int.TryParse(
                property.GetString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var stringValue))
        {
            return stringValue;
        }

        if (property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt32(out var numberValue))
        {
            return numberValue;
        }

        return 0;
    }

    private async Task<JsonElement> GetResultsAsync(
        string endpoint,
        IDictionary<string, string?> parameters,
        CancellationToken cancellationToken)
    {
        EnsureConfigured();
        parameters["client_id"] = _options.ClientId;
        parameters["format"] = "json";

        var requestUri = QueryHelpers.AddQueryString(endpoint, parameters);
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("results", out var results) ||
            results.ValueKind != JsonValueKind.Array)
        {
            using var emptyResults = JsonDocument.Parse("[]");
            return emptyResults.RootElement.Clone();
        }

        return results.Clone();
    }

    private static void ValidateLimit(int limit)
    {
        if (limit <= 0 || limit > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be between 1 and 200.");
        }
    }

    private static DateTime? ParseDate(
        JsonElement element,
        string propertyName)
    {
        var value = GetString(
            element,
            propertyName);

        if (string.IsNullOrWhiteSpace(value))
            return null;

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
            ? date
            : null;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(
                _options.ClientId))
        {
            throw new InvalidOperationException(
                "Jamendo:ClientId is not configured.");
        }
    }
}
