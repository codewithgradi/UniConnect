using UniConnect.Domain.Enums;

public class Announcement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public UserType TargetAudience { get; set; } 
    public DateTime BroadcastAtUtc { get; set; } = DateTime.UtcNow;
}
