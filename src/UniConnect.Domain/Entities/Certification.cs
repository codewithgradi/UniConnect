using UniConnect.Domain.Entities;

public class Certification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string? CredentialUrl { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}
