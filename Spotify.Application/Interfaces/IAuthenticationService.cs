using Spotify.Application.DTOs.Auth;
using Spotify.Application.DTOs.ForgotPassword;
using Spotify.Application.DTOs.License;

namespace Spotify.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<RegisterResult> RegisterAdminAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAdminAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<GoogleSignInResult> GoogleSignInAsync(
        GoogleExternalUser googleUser,
        CancellationToken cancellationToken = default);

    Task<GoogleSignInResult> CompleteGoogleRegistrationAsync(
        GoogleCompleteRegistrationRequest request,
        CancellationToken cancellationToken = default);

    Task<CheckEmailResult> CheckEmailAsync(
        CheckEmailRequest request,
        CancellationToken cancellationToken = default);

    Task<CheckCodeResult> CheckCodeAsync(
        CheckCodeRequest request,
        CancellationToken cancellationToken = default);

    Task<CheckAuthorCodeResult> CheckAuthorCodeAsync(
        CheckAuthorCodeRequest request,
        CancellationToken cancellationToken = default);

    Task<NewPasswordResult> NewPasswordAsync(
        NewPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<LicenseResult> SendActivationLicenseAsync(
        LicenseDto request,
        CancellationToken cancellationToken = default);

    Task<MeResult> MeAsync(
        CancellationToken cancellationToken = default);

    Task<LogoutResult> LogoutAsync(
        CancellationToken cancellationToken = default);
}