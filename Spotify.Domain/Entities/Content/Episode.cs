using Spotify.Domain.Entities.Content;

public class Episode : AudioContent
{
    public Guid PodcastId { get; set; }
    public int? SeqNumber { get; set; }
}