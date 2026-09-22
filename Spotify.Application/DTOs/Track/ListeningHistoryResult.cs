using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record ListeningHistoryResult(
        bool success,
        IEnumerable<ListeningHistoryResponse> listeningHistory,
        string error)
    {
        public static ListeningHistoryResult Success(IEnumerable<ListeningHistoryResponse> listeningHistory)
        {
            return new ListeningHistoryResult(true, listeningHistory, string.Empty);
        }

        public static ListeningHistoryResult Failure(string error)
        {
            return new ListeningHistoryResult(false, Array.Empty<ListeningHistoryResponse>(), error);
        }
    }
}
