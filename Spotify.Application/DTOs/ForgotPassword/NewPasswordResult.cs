using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.ForgotPassword
{
    public sealed record NewPasswordResult(bool Succeeded, IReadOnlyCollection<string> Errors)
    {
        public static NewPasswordResult Success() =>
            new(true, Array.Empty<string>());
        public static NewPasswordResult Failure(params string[] errors) =>
            new(false, errors);
    }
}
