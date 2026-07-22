using Spotify.Domain.Entities.Content;

public class Episode : AudioContent
{
    public Guid PodcastId { get; set; }

    public Podcast Podcast { get; set; } = null!;
}