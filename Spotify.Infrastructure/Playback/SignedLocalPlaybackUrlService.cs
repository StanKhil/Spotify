using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Spotify.Infrastructure.Authentication;

namespace Spotify.Infrastructure.Playback;

public interface ILocalPlaybackUrlService
{
    string CreateStreamUrl(Guid audioItemId, DateTimeOffset expiresAtUtc);
    bool IsValid(Guid audioItemId, long expiresAtUnixSeconds, string signature);
}

public sealed class SignedLocalPlaybackUrlService : ILocalPlaybackUrlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _signingKey;

    public SignedLocalPlaybackUrlService(
        IHttpContextAccessor httpContextAccessor,
        PlaybackOptions playbackOptions,
        JwtOptions jwtOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _signingKey = string.IsNullOrWhiteSpace(playbackOptions.SigningKey)
            ? jwtOptions.Key
            : playbackOptions.SigningKey;
    }

    public string CreateStreamUrl(Guid audioItemId, DateTimeOffset expiresAtUtc)
    {
        var request = _httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException("An HTTP request is required to create a playback URL.");
        var expiresAt = expiresAtUtc.ToUnixTimeSeconds();
        var signature = CreateSignature(audioItemId, expiresAt);
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return $"{baseUrl}/api/audio/{audioItemId}/stream?expiresAt={expiresAt}&signature={Uri.EscapeDataString(signature)}";
    }

    public bool IsValid(Guid audioItemId, long expiresAtUnixSeconds, string signature)
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresAtUnixSeconds)
        {
            return false;
        }

        var expected = CreateSignature(audioItemId, expiresAtUnixSeconds);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(signature));
    }

    private string CreateSignature(Guid audioItemId, long expiresAtUnixSeconds)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_signingKey));
        var payload = Encoding.UTF8.GetBytes($"{audioItemId:N}:{expiresAtUnixSeconds}");
        return Convert.ToBase64String(hmac.ComputeHash(payload))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
