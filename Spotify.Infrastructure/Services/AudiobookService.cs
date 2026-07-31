using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Audiobook;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AudiobookService : IAudiobookService
{
    private readonly ApplicationContext _context;

    public AudiobookService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AudiobookResponse>> GetAudiobooksAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Audiobooks
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AudiobookResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.AuthorContentId, x.GenreId, x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AudiobookResponse?> GetAudiobookByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Audiobooks
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => new AudiobookResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.AuthorContentId, x.GenreId, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateAudiobookResult> CreateAudiobookAsync(
        CreateAudiobookRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateAudiobookResult.Failure("The specified audio item was not found.");
        }

        if (!await _context.AuthorContents.AnyAsync(x => x.Id == request.AuthorContentId, cancellationToken))
        {
            return CreateAudiobookResult.Failure("The specified author content was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateAudiobookResult.Failure("The specified genre was not found.");
        }

        var audiobook = new Domain.Entities.Content.Audiobook
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            AudioItemId = request.AudioItemId,
            AuthorContentId = request.AuthorContentId,
            GenreId = request.GenreId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Audiobooks.Add(audiobook);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateAudiobookResult.Success(new AudiobookResponse(
            audiobook.Id, audiobook.Name, audiobook.Description, audiobook.DurationSeconds,
            audiobook.AuthorContentId, audiobook.GenreId, audiobook.CreatedAt));
    }

    public async Task<UpdateAudiobookResult> EditAudiobookAsync(
        Guid id, UpdateAudiobookRequest request, CancellationToken cancellationToken = default)
    {
        var audiobook = await _context.Audiobooks
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (audiobook is null)
        {
            return UpdateAudiobookResult.Failure("Audiobook was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAudiobookResult.Failure("The specified genre was not found.");
        }

        audiobook.Name = request.Name.Trim();
        audiobook.Description = request.Description?.Trim();
        audiobook.GenreId = request.GenreId;

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateAudiobookResult.Success(new AudiobookResponse(
            audiobook.Id, audiobook.Name, audiobook.Description, audiobook.DurationSeconds,
            audiobook.AuthorContentId, audiobook.GenreId, audiobook.CreatedAt));
    }

    public async Task<DeleteAudiobookResult> DeleteAudiobookAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var audiobook = await _context.Audiobooks
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (audiobook is null)
        {
            return DeleteAudiobookResult.Failure("Audiobook was not found.");
        }

        audiobook.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteAudiobookResult.Success();
    }
}