using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public MediaType? MediaType { get; set; }
    public string? MediaUrl { get; set; }
    public string? ThumbnailUrl { get; set; } // Transcoded videos/thumbnails
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}

