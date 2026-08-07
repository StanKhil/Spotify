using Spotify.Application.DTOs.Auth;

namespace Spotify.Application.DTOs.License
{
    public sealed record LicenseResult(
        bool Succeeded,
        IReadOnlyCollection<string> Errors)
    {
        public static LicenseResult Success() =>
            new(true, []);

        public static LicenseResult Failure(params string[] errors) =>
            new(false, errors);
    }   
}
