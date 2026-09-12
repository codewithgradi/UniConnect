using UniConnect.Domain.Entities;

public class Reaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string ReactionType { get; set; } = "Like";
}
