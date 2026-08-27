using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetProfileWithDetailsAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> SearchProfilesAsync(string? searchTerm, string? programme, CancellationToken cancellationToken = default);
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
    
    void Update(UserProfile profile);

    // Skill & Endorsement operations within Profile context
    Task AddSkillToProfileAsync(Guid profileId, Guid skillId, CancellationToken cancellationToken = default);
    Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId, CancellationToken cancellationToken = default);
}