using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

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

    public async Task<string?> GetTrackStreamUrlAsync(
        string externalTrackId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new InvalidOperationException("Jamendo:ClientId is not configured.");
        }

        var query = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["format"] = "json",
            ["id"] = externalTrackId,
            ["audioformat"] = _options.AudioFormat,
            ["limit"] = "1"
        };

        var requestUri = QueryHelpers.AddQueryString("tracks/", query);
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("results", out var results) ||
            results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            return null;
        }

        var track = results[0];
        return track.TryGetProperty("audio", out var audio) && audio.ValueKind == JsonValueKind.String
            ? audio.GetString()
            : null;
    }
}
