namespace UniConnect.Domain.Entities;
public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SystemHeadline { get; set; } = string.Empty;
    public string AboutBio { get; set; } = string.Empty;
    public string? GithubUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public string StudentNumber { get; set; }=string.Empty;
    public bool IsPublic { get; set; } = true;
    public string Programme { get; set; } = string.Empty; // For Career Pathway Matching

    public ApplicationUser User { get; set; } = null!;
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public ICollection<Recommendation> GivenRecommendations { get; set; } = new List<Recommendation>();
    public ICollection<Recommendation> ReceivedRecommendations { get; set; } = new List<Recommendation>();
}
