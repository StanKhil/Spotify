using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.City
{
    public sealed record CityResponse(
        Guid Id,
        string Name
    );
}
