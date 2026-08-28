using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Auth
{
    public sealed record LogoutResult(
        Boolean Succeeded,
        string Error
    )
    {
        public static LogoutResult Success() => new LogoutResult(true, null);

        public static LogoutResult Failure(string error) => new LogoutResult(false, error);
    }
}
