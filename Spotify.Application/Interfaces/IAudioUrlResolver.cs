using Spotify.Domain.Entities.Content;

namespace Spotify.Application.Interfaces
{
    public interface IAudioUrlResolver
    {
        Task<string?> ResolveAsync(
            AudioContent content,
            CancellationToken cancellationToken = default);
    }
}
