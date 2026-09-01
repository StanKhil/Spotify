using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Auth
{
    public sealed record MeResult(
        bool Succeeded,
        MeResponse? Me,
        IReadOnlyCollection<string> Errors
    )
    {
        public static MeResult Success(MeResponse me) =>
            new(true, me, []);
        public static MeResult Failure(params string[] errors) =>
            new(false, null, errors);
    }
}
