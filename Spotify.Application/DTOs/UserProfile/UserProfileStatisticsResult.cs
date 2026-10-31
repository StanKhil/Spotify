using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.UserProfile
{
    public sealed record UserProfileStatisticsResult(
        bool success,
        UserProfileStatisticsResponse? statistics,
        string? error)
    {
        public static UserProfileStatisticsResult Success(UserProfileStatisticsResponse statistics)
        {
            return new UserProfileStatisticsResult(true, statistics, string.Empty);
        }
        public static UserProfileStatisticsResult Failure(string? error)
        {
            return new UserProfileStatisticsResult(false, null, error);
        }
    }
}
