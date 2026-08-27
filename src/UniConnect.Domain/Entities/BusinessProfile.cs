namespace UniConnect.Domain.Entities;

public class BusinessProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyRegistrationNumber { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
