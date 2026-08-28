namespace Spotify.Application.Interfaces
{
    public interface ITokenRevocationService
    {
        Task<bool> IsRevokedAsync(
            string jti,
            CancellationToken cancellationToken = default);
    }
}
