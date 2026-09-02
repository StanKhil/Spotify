using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Album
{
    public sealed record AlbumActionResult(
        bool Succeeded,
        AlbumActionResponse? AlbumActionResponse,
        IReadOnlyCollection<string> Errors)
    {
        public static AlbumActionResult Success(AlbumActionResponse albumActionResponse) =>
            new(true, albumActionResponse, []);
        public static AlbumActionResult Failure(params string[] errors) =>
            new(false, null, errors);
    }
}
