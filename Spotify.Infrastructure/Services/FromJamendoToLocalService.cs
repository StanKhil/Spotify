using Microsoft.EntityFrameworkCore;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;


namespace Spotify.Infrastructure.Services
{
    public class FromJamendoToLocalService : IFromJamendoToLocalService
    {
        private readonly ApplicationContext _context;
        private readonly IJamendoService _jamendoService;

        
        public FromJamendoToLocalService(ApplicationContext context,
            IJamendoService jamendoService)
        {
            _context = context;
            _jamendoService = jamendoService;
        }

        public async Task<Album?> GetOrCreateJamendoAlbumAsync(
            string jamendoAlbumId,
            CancellationToken cancellationToken)
        {
            var existingAlbum = await _context.Albums
                .FirstOrDefaultAsync(
                    x => x.Provider == AudioProvider.Jamendo &&
                         x.ExternalContentId == jamendoAlbumId &&
                         x.DeletedAt == null,
                    cancellationToken);

            if (existingAlbum is not null)
                return existingAlbum;

            var jamendoAlbum = await _jamendoService.GetAlbumAsync(
                jamendoAlbumId,
                cancellationToken);

            if (jamendoAlbum is null)
                return null;

            var album = new Album
            {
                Id = Guid.NewGuid(),
                Name = jamendoAlbum.Name,

                Provider = AudioProvider.Jamendo,
                ExternalContentId = jamendoAlbum.Id,
                CoverImage = null,
                IsDraft = false,
            };

            _context.Albums.Add(album);

            await _context.SaveChangesAsync(cancellationToken);

            return album;
        }

        public async Task<Track?> GetOrCreateJamendoTrackAsync(
        string jamendoTrackId,
        CancellationToken cancellationToken)
        {
            var existingTrack = await _context.Tracks
                .Include(x => x.AudioItem)
                .Include(x => x.Authors)
                .FirstOrDefaultAsync(
                    x => x.Provider == AudioProvider.Jamendo &&
                         x.ExternalContentId == jamendoTrackId &&
                         x.DeletedAt == null,
                    cancellationToken);

            if (existingTrack is not null)
                return existingTrack;

            var jamendoTrack = await _jamendoService.GetTrackAsync(
                jamendoTrackId,
                cancellationToken);

            if (jamendoTrack is null)
                return null;

            Album? album = null;

            if (!string.IsNullOrWhiteSpace(jamendoTrack.AlbumId))
            {
                album = await GetOrCreateJamendoAlbumAsync(
                    jamendoTrack.AlbumId,
                    cancellationToken);
            }

            var audioItem = new AudioItem
            {
                Id = Guid.NewGuid(),
                Provider = AudioProvider.Jamendo,
                StorageKey = null,
                ContentType = "audio/mpeg",
                BitrateKbps = null,
                LicenseUrl = null,
                IsDownloadAllowed = false
            };

            var track = new Track
            {
                Id = Guid.NewGuid(),

                Name = jamendoTrack.Name,
                DurationSeconds = jamendoTrack.DurationSeconds,

                Provider = AudioProvider.Jamendo,
                ExternalContentId = jamendoTrack.Id,

                AlbumId = album?.Id,

                AudioItem = audioItem,

                IsDraft = false,
                DeletedAt = null,
                PlaysNumber = 0
            };

            var authorContent = new AuthorContent
            {
                Id = Guid.NewGuid(),
                Item = track
            };

            track.Authors.Add(authorContent);

            _context.Tracks.Add(track);

            await _context.SaveChangesAsync(cancellationToken);

            return track;
        }

        public async Task<Author?> GetOrCreateJamendoAuthorAsync(
            string jamendoAuthorId,
            CancellationToken cancellationToken)
        {
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(
                    x => x.ExternalAuthorId == jamendoAuthorId,
                    cancellationToken);

            if (existingAuthor is not null)
                return existingAuthor;

            var jamendoAuthor = await _jamendoService.GetAuthorAsync(
                jamendoAuthorId,
                cancellationToken);

            if (jamendoAuthor is null)
                return null;

            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = jamendoAuthor.Name,
                ExternalAuthorId = jamendoAuthor.Id
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);
            return author;
        }

        public bool IsJamendoId(string trackId)
        {
            return !string.IsNullOrWhiteSpace(trackId) &&
                   trackId.All(char.IsDigit);
        }
    }
}
