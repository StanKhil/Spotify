namespace Spotify.Domain.Entities.Security;

public class RevokedToken
{
    public Guid Id { get; set; }

    public string Jti { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime RevokedAtUtc { get; set; }
}