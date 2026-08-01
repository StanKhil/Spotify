using Spotify.Application.DTOs.Author;

namespace Spotify.Application.Interfaces;

public interface IAuthorService
{
    Task<IReadOnlyCollection<AuthorResponse>> GetAuthorsAsync(CancellationToken cancellationToken = default);
    Task<AuthorResponse?> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreateAuthorResult> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<DeleteAuthorResult> DeleteAuthorAsync(Guid id, CancellationToken cancellationToken = default);
}