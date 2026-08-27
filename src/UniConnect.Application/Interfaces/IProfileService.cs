using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IProfileService
{
    Task<UserProfileDto> CreateProfileAsync(Guid userId, CreateProfileDto dto, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    Task AddSkillAsync(Guid userId, Guid skillId);
    Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId);
}