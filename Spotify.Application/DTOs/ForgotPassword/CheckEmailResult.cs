using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.ForgotPassword
{
    public sealed record CheckEmailResult(bool Succeeded, IReadOnlyCollection<string> Errors)
    {
        public static CheckEmailResult Success() =>
            new(true, Array.Empty<string>());
        public static CheckEmailResult Failure(params string[] errors) =>
            new(false, errors);
    }
}
