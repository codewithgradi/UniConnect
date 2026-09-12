using UniConnect.Domain.Entities;

public class DirectMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
