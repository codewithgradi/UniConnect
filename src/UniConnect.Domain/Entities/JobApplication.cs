using UniConnect.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;

    public Guid ApplicantId { get; set; }
    public ApplicationUser Applicant { get; set; } = null!;

    public string CvFileUrl { get; set; } = string.Empty;
    public DateTime AppliedAtUtc { get; set; } = DateTime.UtcNow;
}
