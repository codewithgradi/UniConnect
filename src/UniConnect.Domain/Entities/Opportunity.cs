using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;

public class Opportunity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BusinessProfileId { get; set; }
    public BusinessProfile BusinessProfile { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetProgramme { get; set; } = string.Empty;
    public OpportunityStatus Status { get; set; } = OpportunityStatus.PendingApproval;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
