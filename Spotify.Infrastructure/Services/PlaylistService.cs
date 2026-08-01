using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playlist;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class PlaylistService : IPlaylistService
{
    private readonly ApplicationContext _context;

    public PlaylistService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PlaylistResponse>> GetPlaylistsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Playlists
            .OrderBy(x => x.Name)
            .Select(x => new PlaylistResponse(x.Id, x.Name, x.ApplicationUserId, x.Tracks.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<PlaylistResponse?> GetPlaylistByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Playlists
            .Where(x => x.Id == id)
            .Select(x => new PlaylistResponse(x.Id, x.Name, x.ApplicationUserId, x.Tracks.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreatePlaylistResult> CreatePlaylistAsync(
        CreatePlaylistRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.ApplicationUsers.AnyAsync(x => x.Id == request.ApplicationUserId, cancellationToken))
        {
            return CreatePlaylistResult.Failure("The specified user was not found.");
        }

        var playlist = new Domain.Entities.Content.Playlist
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ApplicationUserId = request.ApplicationUserId
        };

        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatePlaylistResult.Success(new PlaylistResponse(playlist.Id, playlist.Name, playlist.ApplicationUserId, 0));
    }

    public async Task<UpdatePlaylistResult> EditPlaylistAsync(
        Guid id, UpdatePlaylistRequest request, CancellationToken cancellationToken = default)
    {
        var playlist = await _context.Playlists.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (playlist is null)
        {
            return UpdatePlaylistResult.Failure("Playlist was not found.");
        }

        playlist.Name = request.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        var tracksCount = await _context.Tracks.CountAsync(x => x.PlaylistId == id, cancellationToken);

        return UpdatePlaylistResult.Success(new PlaylistResponse(playlist.Id, playlist.Name, playlist.ApplicationUserId, tracksCount));
    }

    public async Task<DeletePlaylistResult> DeletePlaylistAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var playlist = await _context.Playlists.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (playlist is null)
        {
            return DeletePlaylistResult.Failure("Playlist was not found.");
        }

        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync(cancellationToken);

        return DeletePlaylistResult.Success();
    }
}