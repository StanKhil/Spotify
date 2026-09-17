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
            .Select(x => new PlaylistResponse(x.Id, x.Name, x.ApplicationUserId, x.PlaylistTracks.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<PlaylistResponse?> GetPlaylistByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Playlists
            .Where(x => x.Id == id)
            .Select(x => new PlaylistResponse(x.Id, x.Name, x.ApplicationUserId, x.PlaylistTracks.Count))
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

        var tracksCount = await _context.PlaylistTracks.CountAsync(x => x.PlaylistId == id, cancellationToken);

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

    public async Task<IReadOnlyCollection<PlaylistTrackResponse>> GetPlaylistTracksAsync(
        Guid playlistId, CancellationToken cancellationToken = default)
    {
        return await _context.PlaylistTracks
            .Where(x => x.PlaylistId == playlistId)
            .OrderBy(x => x.Position)
            .Select(x => new PlaylistTrackResponse(x.TrackId, x.Track.Name, x.Position, x.AddedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<AddTrackToPlaylistResult> AddTrackToPlaylistAsync(
        Guid playlistId, AddTrackToPlaylistRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.Playlists.AnyAsync(x => x.Id == playlistId, cancellationToken))
        {
            return AddTrackToPlaylistResult.Failure("Playlist was not found.");
        }

        var track = await _context.Tracks
            .FirstOrDefaultAsync(x => x.Id == request.TrackId && x.DeletedAt == null, cancellationToken);

        if (track is null)
        {
            return AddTrackToPlaylistResult.Failure("The specified track was not found.");
        }

        var alreadyExists = await _context.PlaylistTracks
            .AnyAsync(x => x.PlaylistId == playlistId && x.TrackId == request.TrackId, cancellationToken);

        if (alreadyExists)
        {
            return AddTrackToPlaylistResult.Failure("This track is already in the playlist.");
        }

        var maxPosition = await _context.PlaylistTracks
            .Where(x => x.PlaylistId == playlistId)
            .Select(x => (int?)x.Position)
            .MaxAsync(cancellationToken) ?? -1;

        var playlistTrack = new Domain.Entities.Content.PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = request.TrackId,
            Position = maxPosition + 1,
            AddedAt = DateTime.UtcNow
        };

        _context.PlaylistTracks.Add(playlistTrack);
        await _context.SaveChangesAsync(cancellationToken);

        return AddTrackToPlaylistResult.Success(new PlaylistTrackResponse(
            track.Id, track.Name, playlistTrack.Position, playlistTrack.AddedAt));
    }

    public async Task<RemoveTrackFromPlaylistResult> RemoveTrackFromPlaylistAsync(
        Guid playlistId, Guid trackId, CancellationToken cancellationToken = default)
    {
        var playlistTrack = await _context.PlaylistTracks
            .FirstOrDefaultAsync(x => x.PlaylistId == playlistId && x.TrackId == trackId, cancellationToken);

        if (playlistTrack is null)
        {
            return RemoveTrackFromPlaylistResult.Failure("This track is not in the playlist.");
        }

        _context.PlaylistTracks.Remove(playlistTrack);
        await _context.SaveChangesAsync(cancellationToken);

        return RemoveTrackFromPlaylistResult.Success();
    }
}