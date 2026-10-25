using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
using Spotify.Application.DTOs.AuthorUpload;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AuthorUploadService : IAuthorUploadService
{
    private const string ForbiddenMessage = "Only authors can manage content.";
    private const string AlbumNotFound = "Album was not found";
    private const string TrackNotFound = "Track was not found";

    private readonly ApplicationContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAudioMetadataService _audioMetadataService;
    private readonly IAudioUrlResolver _audioUrlResolver;

    public AuthorUploadService(
        ApplicationContext context,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IFileStorageService fileStorageService,
        IAudioMetadataService audioMetadataService,
        IAudioUrlResolver audioUrlResolver)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _fileStorageService = fileStorageService;
        _audioMetadataService = audioMetadataService;
        _audioUrlResolver = audioUrlResolver;
    }

    public async Task<GetMyAuthorAlbumsResult> GetMyAlbumsAsync(CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return GetMyAuthorAlbumsResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var albums = await OwnedAlbums(authorId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AlbumResponse(
                x.Id.ToString(), x.Name, x.Description, x.DurationSeconds,
                x.ImageItemId ?? Guid.Empty, x.IsDraft, x.GenreId, x.CreatedAt, x.ImageItem!.ImageList))
            .ToListAsync(cancellationToken);

        return GetMyAuthorAlbumsResult.Success(albums);
    }

    public async Task<GetMyAuthorTracksResult> GetMyTracksAsync(CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return GetMyAuthorTracksResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var tracks = await OwnedTracks(authorId)
            .Include(x => x.TrackTags)
            .Include(x => x.AudioItem)
            .Include(x => x.ImageItem)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var responses = new List<TrackResponse>(tracks.Count);

        foreach (var track in tracks)
        {
            var audioUrl = await _audioUrlResolver.ResolveAsync(track, cancellationToken);
            responses.Add(MapTrack(track, authorId, track.ImageItem?.ImageList) with { AudioUrl = audioUrl });
        }

        return GetMyAuthorTracksResult.Success(responses);
    }

    public async Task<GetMyAuthorTracksResult> GetAlbumTracksAsync(
        Guid albumId, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return GetMyAuthorTracksResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var album = await OwnedAlbums(authorId)
            .Where(x => x.Id == albumId)
            .Select(x => new { x.Id, ImageUrl = x.ImageItem != null ? x.ImageItem.ImageList : null })
            .FirstOrDefaultAsync(cancellationToken);

        if (album is null)
        {
            return GetMyAuthorTracksResult.Failure(AuthorUploadFailure.NotFound, AlbumNotFound);
        }

        var tracks = await _context.Tracks
            .Where(x => x.DeletedAt == null && x.AlbumId == album.Id)
            .Include(x => x.TrackTags)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return GetMyAuthorTracksResult.Success(
            tracks.Select(x => MapTrack(x, authorId, album.ImageUrl)).ToList());
    }

    public async Task<CreateAuthorAlbumResult> CreateAlbumAsync(
        AuthorAlbumUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return CreateAuthorAlbumResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var albumName = request.Name.Trim();

        if (albumName.Length == 0)
        {
            return CreateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "Album name is required");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "The specified genre was not found");
        }

        if (await OwnedAlbums(authorId).AnyAsync(x => x.Name == albumName, cancellationToken))
        {
            return CreateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "You already have an album with this name");
        }

        var cover = await _mediaService.UploadImageAsync(
            request.Cover.Content, request.Cover.FileName, request.Cover.ContentType, cancellationToken);

        if (!cover.Succeeded || cover.ItemId is not Guid coverId)
        {
            return CreateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, cover.Errors.ToArray());
        }

        var album = new Album
        {
            Id = Guid.NewGuid(),
            Name = albumName,
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            ImageItemId = coverId,
            GenreId = request.GenreId,
            IsDraft = true,
            CreatedAt = DateTime.UtcNow
        };

        album.AuthorContent = NewAuthorContent(album, authorId);
        _context.Albums.Add(album);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _context.ChangeTracker.Clear();
            await DeleteImageItemAsync(coverId);
            throw;
        }

        return CreateAuthorAlbumResult.Success(await MapAlbumAsync(album, cancellationToken));
    }

    public async Task<UpdateAuthorAlbumResult> UpdateAlbumAsync(
        AuthorAlbumUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var album = await OwnedAlbums(authorId)
            .Include(x => x.Tracks)
            .FirstOrDefaultAsync(x => x.Id == request.AlbumId, cancellationToken);

        if (album is null)
        {
            return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.NotFound, AlbumNotFound);
        }

        var albumName = request.Name.Trim();

        if (albumName.Length == 0)
        {
            return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "Album name is required");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "The specified genre was not found");
        }

        if (await OwnedAlbums(authorId).AnyAsync(x => x.Id != album.Id && x.Name == albumName, cancellationToken))
        {
            return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "You already have an album with this name");
        }

        var oldImageId = album.ImageItemId;
        Guid? newImageId = null;

        if (request.Cover is not null)
        {
            var cover = await _mediaService.UploadImageAsync(
                request.Cover.Content, request.Cover.FileName, request.Cover.ContentType, cancellationToken);

            if (!cover.Succeeded || cover.ItemId is not Guid coverId)
            {
                return UpdateAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, cover.Errors.ToArray());
            }

            newImageId = coverId;
        }

        album.Name = albumName;
        album.Description = request.Description?.Trim();
        album.GenreId = request.GenreId;

        if (newImageId is Guid replacement)
        {
            album.ImageItemId = replacement;

            foreach (var track in album.Tracks.Where(x => x.DeletedAt == null))
            {
                track.ImageItemId = replacement;
            }
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _context.ChangeTracker.Clear();

            if (newImageId is Guid uploaded)
            {
                await DeleteImageItemAsync(uploaded);
            }

            throw;
        }

        if (newImageId is not null && oldImageId is Guid previous)
        {
            await DeleteImageItemIfOrphanedAsync(previous, cancellationToken);
        }

        return UpdateAuthorAlbumResult.Success(await MapAlbumAsync(album, cancellationToken));
    }

    public async Task<DeleteAuthorAlbumResult> DeleteAlbumAsync(
        Guid albumId, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return DeleteAuthorAlbumResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var album = await OwnedAlbums(authorId)
            .Include(x => x.Tracks)
                .ThenInclude(t => t.AudioItem)
            .FirstOrDefaultAsync(x => x.Id == albumId, cancellationToken);

        if (album is null)
        {
            return DeleteAuthorAlbumResult.Failure(AuthorUploadFailure.NotFound, AlbumNotFound);
        }

        var now = DateTime.UtcNow;
        var storageKeys = new List<string>();

        foreach (var track in album.Tracks.Where(x => x.DeletedAt == null))
        {
            if (track.AudioItem is not null)
            {
                if (!string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
                {
                    storageKeys.Add(track.AudioItem.StorageKey);
                }

                _context.AudioItems.Remove(track.AudioItem);
            }

            track.DeletedAt = now;
        }

        var imageId = album.ImageItemId;
        album.DeletedAt = now;

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var key in storageKeys)
        {
            await _fileStorageService.DeleteAsync(key, cancellationToken);
        }

        if (imageId is Guid cover)
        {
            await DeleteImageItemIfOrphanedAsync(cover, cancellationToken);
        }

        return DeleteAuthorAlbumResult.Success();
    }

    public async Task<PublishAuthorAlbumResult> PublishAlbumAsync(
        Guid albumId, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return PublishAuthorAlbumResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var album = await OwnedAlbums(authorId)
            .Include(x => x.Tracks)
            .FirstOrDefaultAsync(x => x.Id == albumId, cancellationToken);

        if (album is null)
        {
            return PublishAuthorAlbumResult.Failure(AuthorUploadFailure.NotFound, AlbumNotFound);
        }

        if (album.ImageItemId is null)
        {
            return PublishAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "Album must have a cover");
        }

        var activeTracks = album.Tracks.Where(x => x.DeletedAt == null).ToList();

        if (activeTracks.Count == 0)
        {
            return PublishAuthorAlbumResult.Failure(AuthorUploadFailure.Invalid, "Album must contain at least one track");
        }

        album.IsDraft = false;
        foreach (var track in activeTracks)
        {
            track.IsDraft = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return PublishAuthorAlbumResult.Success(await MapAlbumAsync(album, cancellationToken));
    }

    public async Task<AddAuthorTrackResult> AddTrackAsync(
        AuthorTrackUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var album = await OwnedAlbums(authorId)
            .Where(x => x.Id == request.AlbumId)
            .Select(x => new { x.Id, x.ImageItemId, ImageUrl = x.ImageItem != null ? x.ImageItem.ImageList : null })
            .FirstOrDefaultAsync(cancellationToken);

        if (album is null)
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.NotFound, AlbumNotFound);
        }

        var trackName = request.Name.Trim();

        if (trackName.Length == 0)
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "Track name is required");
        }

        if (await _context.Tracks.AnyAsync(
                x => x.DeletedAt == null && x.AlbumId == album.Id && x.Name == trackName,
                cancellationToken))
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "A track with this name already exists in this album");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "The specified genre was not found");
        }

        if (request.MoodId is Guid moodId &&
            !await _context.Moods.AnyAsync(x => x.Id == moodId, cancellationToken))
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "The specified mood was not found");
        }

        var audio = await _mediaService.UploadAudioAsync(
            request.Audio.Content, request.Audio.FileName, request.Audio.ContentType, cancellationToken);

        if (!audio.Succeeded || audio.ItemId is not Guid audioItemId || audio.Url is null)
        {
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, audio.Errors.ToArray());
        }

        int durationSeconds;
        try
        {
            durationSeconds = await _audioMetadataService.GetDurationSecondsAsync(audio.Url, cancellationToken);
        }
        catch
        {
            await DeleteAudioItemAsync(audioItemId);
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "Could not read the audio file");
        }

        if (durationSeconds <= 0)
        {
            await DeleteAudioItemAsync(audioItemId);
            return AddAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "Could not read the audio file");
        }

        var track = new Track
        {
            Id = Guid.NewGuid(),
            Name = trackName,
            Description = request.Description?.Trim(),
            DurationSeconds = durationSeconds,
            AlbumId = album.Id,
            MoodId = request.MoodId,
            GenreId = request.GenreId,
            AudioItemId = audioItemId,
            ImageItemId = album.ImageItemId,
            IsAdult = request.IsAdult,
            IsDraft = request.IsDraft,
            PlaysNumber = 0,
            CreatedAt = DateTime.UtcNow
        };

        track.AuthorContent = NewAuthorContent(track, authorId);
        _context.Tracks.Add(track);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _context.ChangeTracker.Clear();
            await DeleteAudioItemAsync(audioItemId);
            throw;
        }

        return AddAuthorTrackResult.Success(MapTrack(track, authorId, album.ImageUrl));
    }

    public async Task<UpdateAuthorTrackResult> UpdateTrackAsync(
        AuthorTrackUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var track = await OwnedTracks(authorId)
            .Include(x => x.TrackTags)
            .FirstOrDefaultAsync(x => x.Id == request.TrackId, cancellationToken);

        if (track is null)
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.NotFound, TrackNotFound);
        }

        var trackName = request.Name.Trim();

        if (trackName.Length == 0)
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "Track name is required");
        }

        if (await _context.Tracks.AnyAsync(
                x => x.DeletedAt == null && x.Id != track.Id && x.AlbumId == track.AlbumId && x.Name == trackName,
                cancellationToken))
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "A track with this name already exists in this album");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "The specified genre was not found");
        }

        if (request.MoodId is Guid moodId &&
            !await _context.Moods.AnyAsync(x => x.Id == moodId, cancellationToken))
        {
            return UpdateAuthorTrackResult.Failure(AuthorUploadFailure.Invalid, "The specified mood was not found");
        }

        track.Name = trackName;
        track.Description = request.Description?.Trim();
        track.GenreId = request.GenreId;
        track.MoodId = request.MoodId;
        track.IsAdult = request.IsAdult;
        track.IsDraft = request.IsDraft;

        await _context.SaveChangesAsync(cancellationToken);

        var imageUrl = track.ImageItemId is Guid imageItemId
            ? await _context.ImageItems
                .Where(x => x.Id == imageItemId)
                .Select(x => x.ImageList)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        return UpdateAuthorTrackResult.Success(MapTrack(track, authorId, imageUrl));
    }

    public async Task<DeleteAuthorTrackResult> DeleteTrackAsync(
        Guid trackId, CancellationToken cancellationToken = default)
    {
        if (await GetCurrentAuthorIdAsync(cancellationToken) is not Guid authorId)
        {
            return DeleteAuthorTrackResult.Failure(AuthorUploadFailure.Forbidden, ForbiddenMessage);
        }

        var track = await OwnedTracks(authorId)
            .Include(x => x.AudioItem)
            .FirstOrDefaultAsync(x => x.Id == trackId, cancellationToken);

        if (track is null)
        {
            return DeleteAuthorTrackResult.Failure(AuthorUploadFailure.NotFound, TrackNotFound);
        }

        string? storageKey = null;

        if (track.AudioItem is not null)
        {
            storageKey = track.AudioItem.StorageKey;
            _context.AudioItems.Remove(track.AudioItem);
        }

        track.DeletedAt = DateTime.UtcNow;

        if (track.AlbumId is Guid albumId &&
            !await _context.Tracks.AnyAsync(
                x => x.DeletedAt == null && x.AlbumId == albumId && x.Id != track.Id,
                cancellationToken))
        {
            var album = await _context.Albums.FirstOrDefaultAsync(x => x.Id == albumId, cancellationToken);

            if (album is not null)
            {
                album.IsDraft = true;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(storageKey))
        {
            await _fileStorageService.DeleteAsync(storageKey, cancellationToken);
        }

        return DeleteAuthorTrackResult.Success();
    }

    private async Task<Guid?> GetCurrentAuthorIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is not Guid userId)
        {
            return null;
        }

        return await _context.Authors
            .Where(x => x.UserId == userId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<Album> OwnedAlbums(Guid authorId) =>
        _context.Albums.Where(x =>
            x.DeletedAt == null &&
            x.AuthorContent != null &&
            x.AuthorContent.Authors.Any(a => a.AuthorId == authorId));

    private IQueryable<Track> OwnedTracks(Guid authorId) =>
        _context.Tracks.Where(x =>
            x.DeletedAt == null &&
            x.AuthorContent != null &&
            x.AuthorContent.Authors.Any(a => a.AuthorId == authorId));

    private static AuthorContent NewAuthorContent(AudioContent item, Guid authorId)
    {
        var authorContent = new AuthorContent
        {
            Id = Guid.NewGuid(),
            Item = item
        };

        authorContent.Authors.Add(new AuthorContentAuthor
        {
            AuthorContentId = authorContent.Id,
            AuthorId = authorId
        });

        return authorContent;
    }

    private async Task<AlbumResponse> MapAlbumAsync(Album album, CancellationToken cancellationToken)
    {
        var imageUrl = album.ImageItemId is Guid imageItemId
            ? await _context.ImageItems
                .Where(x => x.Id == imageItemId)
                .Select(x => x.ImageList)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        return new AlbumResponse(
            album.Id.ToString(), album.Name, album.Description, album.DurationSeconds,
            album.ImageItemId ?? Guid.Empty, album.IsDraft, album.GenreId, album.CreatedAt, imageUrl);
    }

    private static TrackResponse MapTrack(Track track, Guid authorId, string? imageUrl) => new(
        track.Id.ToString(), track.Name, track.Description, track.DurationSeconds,
        track.AlbumId.ToString(), track.MoodId?.ToString(), track.GenreId, track.PlaysNumber,
        track.IsAdult, track.IsDraft, track.AudioItemId, track.ImageItemId,
        track.TrackTags.Select(x => x.TagId).ToList(), track.CreatedAt, null,
        [authorId.ToString()], imageUrl);

    private async Task<bool> IsImageItemInUseAsync(Guid imageItemId, CancellationToken cancellationToken)
    {
        if (await _context.Albums.AnyAsync(
                x => x.ImageItemId == imageItemId && x.DeletedAt == null, cancellationToken))
        {
            return true;
        }

        if (await _context.Tracks.AnyAsync(
                x => x.ImageItemId == imageItemId && x.DeletedAt == null, cancellationToken))
        {
            return true;
        }

        if (await _context.Episodes.AnyAsync(
                x => x.ImageItemId == imageItemId && x.DeletedAt == null, cancellationToken))
        {
            return true;
        }

        if (await _context.Audiobooks.AnyAsync(
                x => x.ImageItemId == imageItemId && x.DeletedAt == null, cancellationToken))
        {
            return true;
        }

        return await _context.Moods.AnyAsync(x => x.MoodImageId == imageItemId, cancellationToken);
    }

    private async Task DeleteImageItemIfOrphanedAsync(Guid imageItemId, CancellationToken cancellationToken)
    {
        if (await IsImageItemInUseAsync(imageItemId, cancellationToken))
        {
            return;
        }

        await DeleteImageItemAsync(imageItemId);
    }

    private async Task DeleteImageItemAsync(Guid imageItemId)
    {
        var item = await _context.ImageItems.FirstOrDefaultAsync(x => x.Id == imageItemId);

        if (item is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(item.ImageList))
        {
            await _fileStorageService.DeleteAsync(item.ImageList);
        }

        _context.ImageItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    private async Task DeleteAudioItemAsync(Guid audioItemId)
    {
        var item = await _context.AudioItems.FirstOrDefaultAsync(x => x.Id == audioItemId);

        if (item is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(item.StorageKey))
        {
            await _fileStorageService.DeleteAsync(item.StorageKey);
        }

        _context.AudioItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}