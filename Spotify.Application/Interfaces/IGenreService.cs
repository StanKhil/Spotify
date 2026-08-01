using Spotify.Application.DTOs.Genre;

namespace Spotify.Application.Interfaces;

public interface IGenreService
{
    Task<IReadOnlyCollection<GenreResponse>> GetGenresAsync(
        CancellationToken cancellationToken = default);

    Task<CreateGenreResult> CreateGenreAsync(
        CreateGenreRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateGenreResult> EditGenreAsync(
        string id,
        UpdateGenreRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteGenreResult> DeleteGenreAsync(
        string id,
        CancellationToken cancellationToken = default);
}