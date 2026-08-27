using UniConnect.Application.DTOs;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProfileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null) return null;

        return new UserProfileDto(
            profile.Id,
            profile.UserId.ToString(), // <-- Added .ToString() here
            profile.FirstName,
            profile.LastName,
            profile.SystemHeadline,
            profile.AboutBio,
            profile.Programme
        );
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        profile.FirstName = dto.FirstName;
        profile.LastName = dto.LastName;
        profile.SystemHeadline = dto.Headline;
        profile.AboutBio = dto.Bio;

        _unitOfWork.UserProfiles.Update(profile);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddSkillAsync(Guid userId, Guid skillId)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        await _unitOfWork.UserProfiles.AddSkillToProfileAsync(profile.Id, skillId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId)
    {
        await _unitOfWork.UserProfiles.EndorseSkillAsync(targetProfileId, skillId, endorsedByUserId);
        await _unitOfWork.SaveChangesAsync();
    }
}