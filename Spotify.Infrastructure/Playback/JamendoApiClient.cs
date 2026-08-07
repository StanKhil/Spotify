using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Spotify.Application.DTOs.Jamendo;

namespace Spotify.Infrastructure.Playback;

public sealed class JamendoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JamendoOptions _options;

    public JamendoApiClient(HttpClient httpClient, JamendoOptions options)
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

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["search"] = query,
            ["audioformat"] = _options.AudioFormat,
            ["limit"] = limit.ToString()
        };

        var requestUri = QueryHelpers.AddQueryString("tracks/", parameters);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("results", out var results))
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

        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["id"] = trackId,
            ["audioformat"] = _options.AudioFormat,
            ["limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString("tracks/", parameters);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("results", out var results) ||
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
        var track = await GetTrackAsync(trackId, cancellationToken);

        return track?.AudioUrl;
    }

    public async Task<IReadOnlyCollection<JamendoAlbumDto>> SearchAlbumsAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["search"] = query,
            ["limit"] = limit.ToString()
        };
        var requestUri = QueryHelpers.AddQueryString("albums/", parameters);
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (!document.RootElement.TryGetProperty("results", out var results))
        {
            return [];
        }
        var albums = new List<JamendoAlbumDto>();
        foreach (var item in results.EnumerateArray())
        {
            albums.Add(new JamendoAlbumDto(
                Id: GetString(item, "id"),
                Name: GetString(item, "name"),
                ArtistName: GetString(item, "artist_name"),
                ArtistId: GetString(item, "artist_id"),
                ImageUrl: GetString(item, "image"),
                TracksCount: GetInt(item, "tracks_count"),
                ReleaseDate: DateTime.TryParse(GetString(item, "releasedate"), out var releaseDate) ? releaseDate : null
                ));
        }
        return albums;
    }


    public async Task<JamendoAlbumDto?> GetAlbumAsync(
        string albumId,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["id"] = albumId,
            ["limit"] = "1"
        };
        var requestUri = QueryHelpers.AddQueryString("albums/", parameters);
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (!document.RootElement.TryGetProperty("results", out var results) ||
            results.GetArrayLength() == 0)
        {
            return null;
        }
        var item = results[0];
        return new JamendoAlbumDto(
            Id: GetString(item, "id"),
            Name: GetString(item, "name"),
            ArtistName: GetString(item, "artist_name"),
            ArtistId: GetString(item, "artist_id"),
            ImageUrl: GetString(item, "image"),
            TracksCount: GetInt(item, "tracks_count"),
            ReleaseDate: DateTime.TryParse(GetString(item, "releasedate"), out var releaseDate) ? releaseDate : null
            );
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new InvalidOperationException("Jamendo:ClientId is not configured.");
        }
    }

    private static JamendoTrackDto MapTrack(JsonElement track)
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

    private static string GetString(JsonElement element, string property)
    {
        return element.TryGetProperty(property, out var value) &&
               value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;
    }

    private static int GetInt(JsonElement element, string property)
    {
        return element.TryGetProperty(property, out var value) &&
               value.TryGetInt32(out var result)
            ? result
            : 0;
    }
}