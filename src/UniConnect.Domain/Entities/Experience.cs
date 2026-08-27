namespace UniConnect.Domain.Entities;

public class Experience
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}
