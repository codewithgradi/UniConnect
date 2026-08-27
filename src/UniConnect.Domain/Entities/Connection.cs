using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;

public class Connection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RequesterId { get; set; }
    public ApplicationUser Requester { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; } = null!;

    public ConnectionStatus Status { get; set; } = ConnectionStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
