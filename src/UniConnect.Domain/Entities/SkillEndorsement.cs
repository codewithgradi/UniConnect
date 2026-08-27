using UniConnect.Domain.Entities;
namespace UniConnect.Domain.Entities;

public class SkillEndorsement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserSkillUserProfileId { get; set; }
    public Guid UserSkillSkillId { get; set; }
    public UserSkill UserSkill { get; set; } = null!;

    public Guid EndorsedByUserId { get; set; }
    public ApplicationUser EndorsedBy { get; set; } = null!;
}