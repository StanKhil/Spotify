using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;   
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services
{
    public class AlbumActionService : IAlbumActionService
    {
        private readonly ApplicationContext _context;
        private readonly IFromJamendoToLocalService _fromJamendoToLocalService;
        public AlbumActionService(
            ApplicationContext context,
            IFromJamendoToLocalService fromJamendoToLocalService)
        {
            _context = context;
            _fromJamendoToLocalService = fromJamendoToLocalService;
        }

        public async Task<GetLikedAlbumsResult> GetLikedAlbumsAsync(int maxPerPage, int page, Guid userId, CancellationToken cancellationToken = default)
        {
            if (maxPerPage <= 0 || page <= 0)
            {
                return GetLikedAlbumsResult.Failure(
                    "Invalid pagination parameters.");
            }

            var likedAlbumsQuery = _context.Likes
        .AsNoTracking()
        .Where(x => x.ApplicationUserId == userId)
        .Select(x => x.AuthorContent.Item)
        .OfType<Album>()
        .Where(x =>
            x.DeletedAt == null &&
            !x.IsDraft);

            var totalLikedAlbums = await likedAlbumsQuery
                .CountAsync(cancellationToken);

            var likedAlbums = await likedAlbumsQuery
                .OrderByDescending(album => album.CreatedAt)
                .Skip((page - 1) * maxPerPage)
                .Take(maxPerPage)
                .ToListAsync(cancellationToken);

            var likedAlbumResponses = likedAlbums
                .Select(album => new AlbumActionResponse(
                    album.Id,
                    true))
                .ToList();

            return GetLikedAlbumsResult.Success(likedAlbumResponses);
        }

        public async Task<AlbumActionResult?> LikeAsync(string albumId, Guid userId, CancellationToken cancellationToken = default)
        {
            var album = await GetOrCreateAlbumAsync(albumId, cancellationToken);
            if (album == null)
            {
                return AlbumActionResult.Failure("Album not found.");
            }

            var authorContent = await _context.AuthorContents
                .FirstOrDefaultAsync(
            x => x.ItemId == album.Id,
            cancellationToken);

            if (authorContent == null)
            {
                return AlbumActionResult.Failure("Author content not found for the album.");
            }

            var isLiked = await IsLikedAsync(userId, authorContent.Id, cancellationToken);
            if (isLiked)
            {
                return AlbumActionResult.Failure("Album is already liked by the user.");
            }

            var like = new Like
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                AuthorContentId = authorContent.Id
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync(cancellationToken);

            return AlbumActionResult.Success(new AlbumActionResponse(album.Id, true));
        }

        public async Task<AlbumActionResult?> UnlikeAsync(string albumId, Guid userId, CancellationToken cancellationToken = default)
        {
            var album = GetOrCreateAlbumAsync(albumId, cancellationToken).Result;
            
            if(album is null)
            {
                return AlbumActionResult.Failure("Album not found.");
            }

            var authorContent = album.Authors.FirstOrDefault();

            if (authorContent is null)
                return AlbumActionResult.Failure("Author content not found for the album.");

            var like = await _context.Likes
            .FirstOrDefaultAsync(
                x => x.ApplicationUserId == userId &&
                     x.AuthorContentId == authorContent.Id,
                cancellationToken);

            if (like is not null)
            {
                _context.Likes.Remove(like);

                await _context.SaveChangesAsync(
                    cancellationToken);
            }

            return AlbumActionResult.Success(new AlbumActionResponse(album.Id, false));
        }

        private async Task<Album?> GetOrCreateAlbumAsync(
            string albumId,
            CancellationToken cancellationToken)
        {
            if (Guid.TryParse(albumId, out var localAlbumId))
            {
                return await GetLocalAlbumAsync(
                    localAlbumId,
                    cancellationToken);
            }

            if (_fromJamendoToLocalService.IsJamendoId(albumId))
            {
                return await _fromJamendoToLocalService.GetOrCreateJamendoAlbumAsync(
                    albumId,
                    cancellationToken);
            }

            return null;
        }
        private async Task<Album?> GetLocalAlbumAsync(
        Guid albumId,
        CancellationToken cancellationToken)
        {
            return await _context.Albums
                .Include(x => x.Authors)
                .FirstOrDefaultAsync(
                    x => x.Id == albumId &&
                         x.DeletedAt == null &&
                         !x.IsDraft,
                    cancellationToken);
        }

        private async Task<bool> IsLikedAsync(
        Guid userId,
        Guid authorContentId,
        CancellationToken cancellationToken)
        {
            return await _context.Likes
                .AnyAsync(
                    x => x.ApplicationUserId == userId &&
                         x.AuthorContentId == authorContentId,
                    cancellationToken);
        }
    }
}
