using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Author
{
    public sealed record AuthorPageResult(
    bool Succeeded,
    AuthorPageResponse? AuthorPage,
    IReadOnlyCollection<string> Errors)
    {
        public static AuthorPageResult Success(AuthorPageResponse authorPage) => new(true, authorPage, []);

        public static AuthorPageResult Failure(params string[] errors) =>
            new(false, null, errors);
    }
}
