
namespace UniConnect.Domain.Entities;

public class Recommendation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AuthorProfileId { get; set; }
    public UserProfile AuthorProfile { get; set; } = null!;

    public Guid TargetProfileId { get; set; }
    public UserProfile TargetProfile { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}