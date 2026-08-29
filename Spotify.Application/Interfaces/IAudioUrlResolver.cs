using Spotify.Domain.Entities.Content;

namespace Spotify.Application.Interfaces
{
    public interface IAudioUrlResolver
    {
        Task<string?> ResolveAsync(
            AudioItem audioItem,
            CancellationToken cancellationToken = default);
    }
}
