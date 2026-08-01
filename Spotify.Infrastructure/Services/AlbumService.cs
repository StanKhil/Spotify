using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AlbumService : IAlbumService
{
    private readonly ApplicationContext _context;

    public AlbumService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AlbumResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.CoverImageId, x.IsDraft, x.GenreId, x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AlbumResponse?> GetAlbumByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => new AlbumResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.CoverImageId, x.IsDraft, x.GenreId, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateAlbumResult> CreateAlbumAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.CoverImages.AnyAsync(x => x.Id == request.CoverImageId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified cover image was not found.");
        }

        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified audio item was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified genre was not found.");
        }

        var album = new Domain.Entities.Content.Album
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            CoverImageId = request.CoverImageId,
            AudioItemId = request.AudioItemId,
            GenreId = request.GenreId,
            IsDraft = request.IsDraft,
            CreatedAt = DateTime.UtcNow
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateAlbumResult.Success(new AlbumResponse(
            album.Id, album.Name, album.Description, album.DurationSeconds,
            album.CoverImageId, album.IsDraft, album.GenreId, album.CreatedAt));
    }

    public async Task<UpdateAlbumResult> EditAlbumAsync(
        Guid id,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        var album = await _context.Albums
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (album is null)
        {
            return UpdateAlbumResult.Failure("Album was not found.");
        }

        if (!await _context.CoverImages.AnyAsync(x => x.Id == request.CoverImageId, cancellationToken))
        {
            return UpdateAlbumResult.Failure("The specified cover image was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAlbumResult.Failure("The specified genre was not found.");
        }

        album.Name = request.Name.Trim();
        album.Description = request.Description?.Trim();
        album.CoverImageId = request.CoverImageId;
        album.GenreId = request.GenreId;
        album.IsDraft = request.IsDraft;

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateAlbumResult.Success(new AlbumResponse(
            album.Id, album.Name, album.Description, album.DurationSeconds,
            album.CoverImageId, album.IsDraft, album.GenreId, album.CreatedAt));
    }

    public async Task<DeleteAlbumResult> DeleteAlbumAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var album = await _context.Albums
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (album is null)
        {
            return DeleteAlbumResult.Failure("Album was not found.");
        }

        album.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteAlbumResult.Success();
    }
}