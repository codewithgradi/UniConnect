using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IProfileService
{
    Task SaveCvUrlToDbAsync(Guid userId, string url, CancellationToken cancellationToken = default);
    Task<DetailedUserProfileDto> CreateProfileAsync(Guid userId, CreateProfileDto dto, CancellationToken cancellationToken = default);
    Task<DetailedUserProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DetailedUserProfileDto?> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    Task<ExperienceDto> AddExperienceAsync(Guid userId, AddExperienceDto dto, CancellationToken cancellationToken = default);
    Task DeleteExperienceAsync(Guid userId, Guid experienceId, CancellationToken cancellationToken = default);
    Task<CertificationDto> AddCertificationAsync(Guid userId, AddCertificationDto dto, CancellationToken cancellationToken = default);
    Task DeleteCertificationAsync(Guid userId, Guid certificationId, CancellationToken cancellationToken = default);
    Task AddSkillAsync(Guid userId, Guid skillId, CancellationToken cancellationToken = default);
    Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId, CancellationToken cancellationToken = default);
}