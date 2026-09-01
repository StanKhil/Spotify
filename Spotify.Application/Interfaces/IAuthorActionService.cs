using Spotify.Application.DTOs.Author;

namespace Spotify.Application.Interfaces
{
    public interface IAuthorActionService
    {
        Task<AuthorActionResponse?> SubscribeAsync(
            string authorId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AuthorActionResponse?> UnsubscribeAsync(
            string authorId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<SubscribedAuthorsResult> GetSubscribed(
            int maxPerPage,
            int page,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
