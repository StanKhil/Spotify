using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Author;

public sealed class CreateAuthorRequest
{
    [Required]
    public Guid ApplicationUserId { get; init; }
}