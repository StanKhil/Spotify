using MailKit;
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

        Task<Guid?> GetAuthorIdByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AuthorPageResult> GetAuthorPageAsync(
            string authorId,
            int tracksMaxPerPage,
            int tracksPage,
            int albumsMaxPerPage,
            int albumsPage,
            int interestsMaxPerPage,
            int interestsPage,
            int artistsMaxPerPage,
            int artistsPage,
            CancellationToken cancellationToken = default);
    }
}
