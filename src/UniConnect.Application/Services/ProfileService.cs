using System.Security.Cryptography;
using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProfileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> CreateProfileAsync(Guid userId, CreateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var existingProfile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (existingProfile != null)
        {
            throw new InvalidOperationException("Profile already exists for this user.");
        }


        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            StudentNumber = dto.StudentNumber,
            Programme = dto.Programme,
            SystemHeadline = dto.Headline,
            AboutBio = dto.Bio
        };

        await _unitOfWork.UserProfiles.AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Make sure the DTO positional arguments EXACTLY match the record parameter order
        return new UserProfileDto(
            profile.Id,
            profile.UserId.ToString(),
            profile.FirstName,        // Match order of UserProfileDto definition
            profile.LastName,
            profile.SystemHeadline,
            profile.AboutBio,
            profile.Programme,
            profile.StudentNumber
        );
    }
    public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
        if (profile == null) return null;

        return new UserProfileDto(
            profile.Id,
            profile.UserId.ToString(),
            profile.StudentNumber,
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
        profile.StudentNumber = dto.StudentNumber;

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