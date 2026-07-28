
using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.ForgotPassword
{
    public sealed class CheckEmailRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; init; } = string.Empty;
    }
}
