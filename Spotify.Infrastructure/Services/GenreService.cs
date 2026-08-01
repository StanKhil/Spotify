using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Genre;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class GenreService : IGenreService
{
    private readonly ApplicationContext _context;

    public GenreService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<GenreResponse>> GetGenresAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Genres
            .OrderBy(x => x.Name)
            .Select(x => new GenreResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateGenreResult> CreateGenreAsync(
        CreateGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = request.Id.Trim();
        var name = request.Name.Trim();

        if (await _context.Genres.AnyAsync(x => x.Id == id, cancellationToken))
        {
            return CreateGenreResult.Failure("A genre with this id already exists");
        }

        var genre = new Domain.Entities.Content.Genre
        {
            Id = id,
            Name = name
        };

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateGenreResult.Success(new GenreResponse(genre.Id, genre.Name));
    }

    public async Task<UpdateGenreResult> EditGenreAsync(
        string id,
        UpdateGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (genre is null)
        {
            return UpdateGenreResult.Failure("Genre was not found");
        }

        genre.Name = request.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return UpdateGenreResult.Success(new GenreResponse(genre.Id, genre.Name));
    }

    public async Task<DeleteGenreResult> DeleteGenreAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (genre is null)
        {
            return DeleteGenreResult.Failure("Genre was not found");
        }

        var isInUse = await _context.Set<Domain.Entities.Content.AudioContent>()
            .AnyAsync(x => x.GenreId == id, cancellationToken);

        if (isInUse)
        {
            return DeleteGenreResult.Failure("Cannot delete a genre that is used by existing content");
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteGenreResult.Success();
    }
}