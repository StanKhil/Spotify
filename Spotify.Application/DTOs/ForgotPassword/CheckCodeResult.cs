using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.ForgotPassword
{
    public sealed record CheckCodeResult(bool Succeeded, IReadOnlyCollection<string> Errors)
    {
        public static CheckCodeResult Success() =>
            new(true, Array.Empty<string>());
        public static CheckCodeResult Failure(params string[] errors) =>
            new(false, errors);
    }
}
