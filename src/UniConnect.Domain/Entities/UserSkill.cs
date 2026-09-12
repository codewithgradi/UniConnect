namespace UniConnect.Domain.Entities;

public class UserSkill
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public ICollection<SkillEndorsement> Endorsements { get; set; } = new List<SkillEndorsement>();
}