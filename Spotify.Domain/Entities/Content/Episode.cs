using Spotify.Domain.Entities.Content;

public class Episode : AudioContent
{
    public Guid? PodcastId { get; set; }
    public Podcast? Podcast { get; set; }

    public Guid? AudiobookId { get; set; }
    public Audiobook? Audiobook { get; set; }

    public int? SeqNumber { get; set; }
}