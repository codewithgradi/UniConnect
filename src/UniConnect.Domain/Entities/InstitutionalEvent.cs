using UniConnect.Domain.Entities;

public class InstitutionalEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public Guid CreatedByAdminId { get; set; }
    public ApplicationUser CreatedByAdmin { get; set; } = null!;
    public bool IsPublished { get; set; }
}