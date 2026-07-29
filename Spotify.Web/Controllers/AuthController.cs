using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using Spotify.Application.DTOs.Auth;
using Spotify.Application.Interfaces;
using Spotify.Application.DTOs.ForgotPassword;
using ApplicationAuthenticationService = Spotify.Application.Interfaces.IAuthenticationService;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ApplicationAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;

    public AuthController(
        ApplicationAuthenticationService authenticationService,
        IConfiguration configuration)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RegisterAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.User);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(result.Authentication);
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var callbackUrl = Url.ActionLink(
            nameof(GoogleCallback),
            values: null,
            protocol: Request.Scheme)
            ?? throw new InvalidOperationException("Unable to generate the Google callback URL.");

        return Challenge(
            new AuthenticationProperties { RedirectUri = callbackUrl },
            GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(CancellationToken cancellationToken)
    {
        var externalAuthentication = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!externalAuthentication.Succeeded || externalAuthentication.Principal is null)
        {
            return BadRequest("Google authentication failed.");
        }

        try
        {
            var principal = externalAuthentication.Principal;
            var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var displayName = principal.FindFirstValue(ClaimTypes.Name) ?? email;

            if (string.IsNullOrWhiteSpace(providerKey) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Google did not provide the required account information.");
            }

            var result = await _authenticationService.GoogleSignInAsync(
                new GoogleExternalUser(providerKey, email, displayName ?? email),
                cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            if (result.RequiresRegistration)
            {
                var redirectUrl = GetRequiredGoogleFrontendUrl("RegistrationRedirectUrl");
                return Redirect(QueryHelpers.AddQueryString(
                    redirectUrl,
                    "registrationToken",
                    result.RegistrationToken!));
            }

            return RedirectWithAccessToken(result.Authentication!);
        }
        finally
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    [HttpPost("google/complete-registration")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> CompleteGoogleRegistration(
        [FromBody] GoogleCompleteRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.CompleteGoogleRegistrationAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(result.Authentication);
    }

    [HttpPost("forgot-password/check-email")]
    public async Task<ActionResult<CheckEmailResult>> CheckEmail(
        [FromBody] CheckEmailRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.CheckEmailAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("forgot-password/check-code")]
    public async Task<ActionResult<CheckCodeResult>> CheckCode(
        [FromBody] CheckCodeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.CheckCodeAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("forgot-password/new-password")]
    public async Task<ActionResult<NewPasswordResult>> NewPassword(
        [FromBody] NewPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.NewPasswordAsync(request, cancellationToken);

        return Ok(result);

    }

    private IActionResult RedirectWithAccessToken(AuthenticationResponse authentication)
    {
        var redirectUrl = GetRequiredGoogleFrontendUrl("LoginRedirectUrl");
        var fragment = $"accessToken={Uri.EscapeDataString(authentication.AccessToken)}" +
            $"&expiresAtUtc={Uri.EscapeDataString(authentication.ExpiresAtUtc.ToString("O"))}";

        return Redirect($"{redirectUrl}#{fragment}");
    }

    private string GetRequiredGoogleFrontendUrl(string key) =>
        _configuration[$"Google:{key}"]
        ?? throw new InvalidOperationException($"Google:{key} is missing.");
}
