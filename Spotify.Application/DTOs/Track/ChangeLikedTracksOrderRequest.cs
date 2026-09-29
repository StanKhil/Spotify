using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Track;

public sealed record ChangeLikedTracksOrderRequest(
    [property: Range(1, int.MaxValue)] int OldOrder,
    [property: Range(1, int.MaxValue)] int NewOrder);
