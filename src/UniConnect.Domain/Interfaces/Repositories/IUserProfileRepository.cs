using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IUserProfileRepository
{
    
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetProfileWithDetailsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetProfileWithDetailsAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> SearchProfilesAsync(string? searchTerm, string? programme, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
    void Update(UserProfile profile);
    void AddExperience(Experience experience);
    Task<UserProfile?> GetByUserProfileIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void AddCertification(Certification certification);
    Task AddSkillToProfileAsync(Guid profileId, Guid skillId, CancellationToken cancellationToken = default);
    Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId, CancellationToken cancellationToken = default);
    Task SaveCvUrlToDbAsync(Guid profileId,string url, CancellationToken token);
}