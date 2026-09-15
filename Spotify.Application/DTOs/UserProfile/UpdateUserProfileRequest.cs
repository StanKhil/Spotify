using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.UserProfile
{
    public sealed record UpdateUserProfileRequest(
        Guid userId,
        string? userName,
        string? description,
        string? avatarImageId,
        string? coverImageId
        );
}
