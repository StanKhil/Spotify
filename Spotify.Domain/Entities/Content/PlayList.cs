using System.ComponentModel.DataAnnotations.Schema;
using Spotify.Domain.Entities.User;

namespace Spotify.Domain.Entities.Content;

public class Playlist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = [];
}
