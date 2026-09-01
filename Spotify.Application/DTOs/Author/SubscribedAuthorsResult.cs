using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Author
{
    public sealed record SubscribedAuthorsResult(
        bool Succeeded,
        SubscribedAuthorsResponse? Authors,
        IReadOnlyCollection<string> Errors)
    {
        public static SubscribedAuthorsResult Success(SubscribedAuthorsResponse authors) =>
            new(true, authors, []);
        public static SubscribedAuthorsResult Failure(params string[] errors) =>
            new(false, null, errors);
    }
}
