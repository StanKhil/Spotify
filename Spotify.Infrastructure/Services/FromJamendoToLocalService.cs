using Microsoft.EntityFrameworkCore;
using Spotify.Application.Interfaces;
using Spotify.Application.DTOs.Jamendo;
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
                IsDraft = false,
            };

            var authorContent = new AuthorContent
            {
                Id = Guid.NewGuid(),
                Item = album
            };

            _context.Albums.Add(album);
            _context.AuthorContents.Add(authorContent);

            await _context.SaveChangesAsync(cancellationToken);

            return album;
        }

        public async Task<Track?> GetOrCreateJamendoTrackAsync(
        string jamendoTrackId,
        CancellationToken cancellationToken)
        {
            var existingTrack = await _context.Tracks
                .Include(x => x.AudioItem)
                .Include(x => x.ImageItem)
                .Include(x => x.Album)
                .Include(x => x.AuthorContent)
                    .ThenInclude(x => x.Authors)
                        .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(
                    x => x.Provider == AudioProvider.Jamendo &&
                         x.ExternalContentId == jamendoTrackId &&
                         x.DeletedAt == null,
                    cancellationToken);

            if (existingTrack is not null)
            {
                await HydrateMissingJamendoDetailsAsync(
                    existingTrack,
                    cancellationToken);

                return existingTrack;
            }

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
            

            Author? author = null;

            if (!string.IsNullOrWhiteSpace(jamendoTrack.ArtistId))
            {
                author = await GetOrCreateJamendoAuthorAsync(
                    jamendoTrack.ArtistId,
                    cancellationToken);
            }

            var imageItem = new ImageItem
            {
                Id = Guid.NewGuid(),
                ImageList = jamendoTrack.ImageUrl
            };

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

                ImageItem = imageItem,
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

            if (author is not null)
            {
                authorContent.Authors.Add(
                    new AuthorContentAuthor
                    {
                        AuthorContentId = authorContent.Id,
                        AuthorId = author.Id,
                        Author = author,
                        AuthorContent = authorContent
                    });
            }

            track.AuthorContent = authorContent;

            _context.Tracks.Add(track);

            await _context.SaveChangesAsync(cancellationToken);

            return track;
        }

        private async Task HydrateMissingJamendoDetailsAsync(
            Track track,
            CancellationToken cancellationToken)
        {
            var needsAuthor = track.AuthorContent is null ||
                              track.AuthorContent.Authors.Count == 0;
            var needsAlbum = track.Album is null;
            var needsImage = track.ImageItem is null;

            if (!needsAuthor && !needsAlbum && !needsImage ||
                string.IsNullOrWhiteSpace(track.ExternalContentId))
            {
                return;
            }

            var jamendoTrack = await _jamendoService.GetTrackAsync(
                track.ExternalContentId,
                cancellationToken);

            if (jamendoTrack is null)
            {
                return;
            }

            var changed = false;

            if (needsAlbum && !string.IsNullOrWhiteSpace(jamendoTrack.AlbumId))
            {
                var album = await GetOrCreateJamendoAlbumAsync(
                    jamendoTrack.AlbumId,
                    cancellationToken);

                if (album is not null)
                {
                    track.AlbumId = album.Id;
                    track.Album = album;
                    changed = true;
                }
            }

            if (needsImage && !string.IsNullOrWhiteSpace(jamendoTrack.ImageUrl))
            {
                track.ImageItem = new ImageItem
                {
                    Id = Guid.NewGuid(),
                    ImageList = jamendoTrack.ImageUrl
                };
                changed = true;
            }

            if (needsAuthor && !string.IsNullOrWhiteSpace(jamendoTrack.ArtistId))
            {
                var author = await GetOrCreateJamendoAuthorAsync(
                    jamendoTrack.ArtistId,
                    cancellationToken);

                if (author is not null)
                {
                    if (track.AuthorContent is null)
                    {
                        track.AuthorContent = new AuthorContent
                        {
                            Id = Guid.NewGuid(),
                            Item = track
                        };
                    }

                    if (!track.AuthorContent.Authors.Any(x => x.AuthorId == author.Id))
                    {
                        track.AuthorContent.Authors.Add(new AuthorContentAuthor
                        {
                            AuthorContentId = track.AuthorContent.Id,
                            AuthorId = author.Id,
                            Author = author,
                            AuthorContent = track.AuthorContent
                        });
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
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
