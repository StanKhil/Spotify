using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Author
{
    public sealed record SubscribedAuthorsResponse(
        IEnumerable<AuthorResponse> Authors
        );
}
