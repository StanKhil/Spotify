using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Album
{
    public sealed record GetLikedAlbumsResult(
        bool Succeeded,
        IReadOnlyCollection<AlbumActionResponse> Albums,
        IReadOnlyCollection<string> Errors)
    {
        public static GetLikedAlbumsResult Success(IReadOnlyCollection<AlbumActionResponse> albums) =>
            new(true, albums, []);
        public static GetLikedAlbumsResult Failure(params string[] errors) =>
            new(false, [], errors);
    }
}
