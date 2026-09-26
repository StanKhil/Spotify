using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Playlist;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Services;

public sealed class PlaylistService : IPlaylistService
{
    private readonly ApplicationContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFromJamendoToLocalService _fromJamendoToLocalService;

    public PlaylistService(ApplicationContext context,
        IFromJamendoToLocalService fromJamendoToLocalService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fromJamendoToLocalService = fromJamendoToLocalService;
        _currentUserService = currentUserService;
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

        var currentUserId = _currentUserService.UserId;

        if(currentUserId != playlist.ApplicationUserId)
        {
            return UpdatePlaylistResult.Failure("You cannot edit someone else's playlist.");
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

        var currentUserId = _currentUserService.UserId;

        if (currentUserId != playlist.ApplicationUserId)
        {
            return DeletePlaylistResult.Failure("You cannot delete someone else's playlist.");
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
        var playlist = await _context.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId, cancellationToken);
        if (playlist is null)
        {
            return AddTrackToPlaylistResult.Failure("Playlist was not found.");
        }

        var currentUserId = _currentUserService.UserId;

        if (currentUserId != playlist.ApplicationUserId)
        {
            return AddTrackToPlaylistResult.Failure("You cannot edit someone else's playlist.");
        }

        var track = await GetOrCreateTrackAsync(request.TrackId, cancellationToken);

        if (track is null)
        {
            return AddTrackToPlaylistResult.Failure("The specified track was not found.");
        }

        var alreadyExists = await _context.PlaylistTracks
            .AnyAsync(x => x.PlaylistId == playlistId && x.TrackId == track.Id, cancellationToken);

        if (alreadyExists)
        {
            return AddTrackToPlaylistResult.Failure("This track is already in the playlist.");
        }

        var maxPosition = await _context.PlaylistTracks
            .Where(x => x.PlaylistId == playlistId)
            .Select(x => (int?)x.Position)
            .MaxAsync(cancellationToken) ?? -1;

        var playlistTrack = new PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = track.Id,
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
        var playlist = await _context.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId, cancellationToken);
        if (playlist is null)
        {
            return RemoveTrackFromPlaylistResult.Failure("Playlist was not found.");
        }

        var currentUserId = _currentUserService.UserId;

        if (currentUserId != playlist.ApplicationUserId)
        {
            return RemoveTrackFromPlaylistResult.Failure("You cannot edit someone else's playlist.");
        }

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

    private async Task<Track?> GetOrCreateTrackAsync(
    string trackId,
    CancellationToken cancellationToken)
    {
        if (Guid.TryParse(trackId, out var localTrackId))
        {
            return await GetLocalTrackAsync(
                localTrackId,
                cancellationToken);
        }

        if (_fromJamendoToLocalService.IsJamendoId(trackId))
        {
            return await _fromJamendoToLocalService.GetOrCreateJamendoTrackAsync(
                trackId,
                cancellationToken);
        }

        return null;
    }

    private async Task<Track?> GetLocalTrackAsync(
        Guid trackId,
        CancellationToken cancellationToken)
    {
        return await _context.Tracks
            .Include(x => x.AuthorContent)
                .ThenInclude(ac => ac.Authors)
            .FirstOrDefaultAsync(
                x => x.Id == trackId &&
                     x.DeletedAt == null &&
                     !x.IsDraft,
                cancellationToken);
    }
}