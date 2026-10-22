using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Author
{
    public sealed record SubscribedAuthorsResult(
        bool Succeeded,
        IEnumerable<AuthorResponse> Artists,
        IReadOnlyCollection<string> Errors)
    {
        public static SubscribedAuthorsResult Success(IEnumerable<AuthorResponse> Artists) =>
            new(true, Artists, []);
        public static SubscribedAuthorsResult Failure(params string[] errors) =>
            new(false, null, errors);
    }
}
