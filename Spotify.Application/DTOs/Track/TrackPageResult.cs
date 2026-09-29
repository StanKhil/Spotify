using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackPageResult(
        bool success,
        TrackPageResponse? response,
        string error
        )
    {
        public static TrackPageResult Success(TrackPageResponse response)
        {
            return new TrackPageResult(true, response, string.Empty);
        }

        public static TrackPageResult Failure(string error)
        {
            return new TrackPageResult(false, null, error);
        }
    }
}
