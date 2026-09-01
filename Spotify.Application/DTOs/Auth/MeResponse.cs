using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Auth
{
    public sealed record MeResponse(
        Guid Id, 
        string UserName, 
        string Email,
        int FollowersCount,
        int FollowingCount,
        Boolean IsAuthor
    );

}
