using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AlbumService : IAlbumService
{
    private readonly ApplicationContext _context;
    private readonly IFileStorageService _fileStorageService;

    public AlbumService(ApplicationContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Albums
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AlbumResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.ImageItemId ?? Guid.Empty, x.IsDraft, x.GenreId, x.CreatedAt))
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
                x.ImageItemId ?? Guid.Empty, x.IsDraft, x.GenreId, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateAlbumResult> CreateAlbumAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.ImageItems.AnyAsync(x => x.Id == request.CoverImageId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified cover image was not found");
        }

        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified audio item was not found");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateAlbumResult.Failure("The specified genre was not found");
        }

        var album = new Domain.Entities.Content.Album
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            ImageItemId = request.CoverImageId,
            AudioItemId = request.AudioItemId,
            GenreId = request.GenreId,
            IsDraft = request.IsDraft,
            CreatedAt = DateTime.UtcNow
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateAlbumResult.Success(new AlbumResponse(
            album.Id, album.Name, album.Description, album.DurationSeconds,
            album.ImageItemId ?? Guid.Empty, album.IsDraft, album.GenreId, album.CreatedAt));
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
            return UpdateAlbumResult.Failure("Album was not found");
        }

        if (!await _context.ImageItems.AnyAsync(x => x.Id == request.CoverImageId, cancellationToken))
        {
            return UpdateAlbumResult.Failure("The specified cover image was not found");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAlbumResult.Failure("The specified genre was not found");
        }

        album.Name = request.Name.Trim();
        album.Description = request.Description?.Trim();
        album.ImageItemId = request.CoverImageId;
        album.GenreId = request.GenreId;
        album.IsDraft = request.IsDraft;

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateAlbumResult.Success(new AlbumResponse(
            album.Id, album.Name, album.Description, album.DurationSeconds,
            album.ImageItemId ?? Guid.Empty, album.IsDraft, album.GenreId, album.CreatedAt));
    }

    public async Task<DeleteAlbumResult> DeleteAlbumAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var album = await _context.Albums
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (album is null)
        {
            return DeleteAlbumResult.Failure("Album was not found");
        }

        if (album.ImageItemId is Guid imageItemId)
        {
            await DeleteImageItemIfOrphanedAsync(imageItemId, album.Id, cancellationToken);
        }

        album.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteAlbumResult.Success();
    }

    private async Task<bool> IsImageItemInUseAsync(
        Guid imageItemId,
        Guid excludedAlbumId,
        CancellationToken cancellationToken)
    {
        var usedByAlbum = await _context.Albums.AnyAsync(
            x => x.ImageItemId == imageItemId &&
                 x.DeletedAt == null &&
                 x.Id != excludedAlbumId,
            cancellationToken);

        if (usedByAlbum)
        {
            return true;
        }

        var usedByTrack = await _context.Tracks.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByTrack)
        {
            return true;
        }

        var usedByEpisode = await _context.Episodes.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByEpisode)
        {
            return true;
        }

        var usedByAudiobook = await _context.Audiobooks.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByAudiobook)
        {
            return true;
        }

        var usedByMood = await _context.Moods.AnyAsync(
            x => x.MoodImageId == imageItemId,
            cancellationToken);

        return usedByMood;
    }

    private async Task DeleteImageItemIfOrphanedAsync(
        Guid imageItemId,
        Guid excludedAlbumId,
        CancellationToken cancellationToken)
    {
        var isInUse = await IsImageItemInUseAsync(imageItemId, excludedAlbumId, cancellationToken);

        if (isInUse)
        {
            return;
        }

        var imageItem = await _context.ImageItems
            .FirstOrDefaultAsync(x => x.Id == imageItemId, cancellationToken);

        if (imageItem is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(imageItem.ImageList))
        {
            await _fileStorageService.DeleteAsync(imageItem.ImageList, cancellationToken);
        }

        _context.ImageItems.Remove(imageItem);
    }
}