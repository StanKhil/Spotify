using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.ForgotPassword;

public sealed class CheckCodeRequest
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}
