using Microsoft.AspNetCore.Http;
using Spotify.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Jti =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(JwtRegisteredClaimNames.Jti);

    public DateTime? ExpiresAtUtc
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(JwtRegisteredClaimNames.Exp);

            if (!long.TryParse(value, out var exp))
                return null;

            return DateTimeOffset
                .FromUnixTimeSeconds(exp)
                .UtcDateTime;
        }
    }
}