using Spotify.Application.DTOs.Author;

namespace Spotify.Application.Interfaces
{
    public interface IAuthorActionService
    {
        Task<AuthorActionResponse?> SubscribeAsync(
            Guid authorId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AuthorActionResponse?> UnsubscribeAsync(
            Guid authorId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
