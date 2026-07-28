using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Auth;
using Spotify.Application.Interfaces;
using Spotify.Application.DTOs.ForgotPassword;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
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

            return ValidationProblem(ModelState, statusCode: StatusCodes.Status401Unauthorized);
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
}
