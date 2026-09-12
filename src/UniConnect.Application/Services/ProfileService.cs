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

    public async Task<DetailedUserProfileDto> CreateProfileAsync(Guid userId, CreateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var existingProfile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
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

        return MapToDetailedDto(profile);
    }

    public async Task<DetailedUserProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var detailedProfile = await _unitOfWork.UserProfiles.GetProfileWithDetailsByUserIdAsync(userId, cancellationToken);
        return detailedProfile == null ? null : MapToDetailedDto(detailedProfile);
    }

    public async Task<DetailedUserProfileDto?> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetProfileWithDetailsAsync(profileId, cancellationToken);
        return profile == null ? null : MapToDetailedDto(profile);
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        profile.FirstName = dto.FirstName;
        profile.LastName = dto.LastName;
        profile.SystemHeadline = dto.Headline;
        profile.AboutBio = dto.Bio;
        profile.StudentNumber = dto.StudentNumber;
        profile.Programme = dto.Programme;

        _unitOfWork.UserProfiles.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task<ExperienceDto> AddExperienceAsync(Guid userId, AddExperienceDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        // Instantiate a clean, new entity instance directly
        var experience = new Experience
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Title = dto.Title,
            Company = dto.CompanyName,
            StartDate = dto.StartDate,
            EndDate = dto.IsCurrent ? null : dto.EndDate,
            IsCurrent = dto.IsCurrent
        };

        profile.Experiences ??= new List<Experience>();
        profile.Experiences.Add(experience);

        _unitOfWork.UserProfiles.AddExperience(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExperienceDto(
            experience.Id,
            experience.Title,
            experience.Company,
            experience.StartDate,
            experience.EndDate,
            experience.IsCurrent
        );
    }

    public async Task DeleteExperienceAsync(Guid userId, Guid experienceId, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetProfileWithDetailsAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        var exp = profile.Experiences.FirstOrDefault(e => e.Id == experienceId);
        if (exp != null)
        {
            profile.Experiences.Remove(exp);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<CertificationDto> AddCertificationAsync(Guid userId, AddCertificationDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        var certification = new Certification
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Name = dto.Name,
            IssuingOrganization = dto.IssuingOrganization,
            IssueDate = dto.IssueDate,
            CredentialUrl = dto.CredentialUrl
        };

        _unitOfWork.UserProfiles.AddCertification(certification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CertificationDto(
            certification.Id,
            certification.Name,
            certification.IssuingOrganization,
            certification.IssueDate,
            certification.CredentialUrl
        );
    }
    public async Task DeleteCertificationAsync(Guid userId, Guid certificationId, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetProfileWithDetailsAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        var cert = profile.Certifications.FirstOrDefault(c => c.Id == certificationId);
        if (cert != null)
        {
            profile.Certifications.Remove(cert);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
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

    private static DetailedUserProfileDto MapToDetailedDto(UserProfile p)
    {
        return new DetailedUserProfileDto(
            p.Id,
            p.UserId.ToString(),
            p.FirstName,
            p.LastName,
            p.StudentNumber,
            p.Programme,
            p.SystemHeadline,
            p.AboutBio,
            p.CvFileUrl,
            p.Experiences?.Select(e => new ExperienceDto(e.Id, e.Title, e.Company, e.StartDate, e.EndDate, e.IsCurrent)) ?? Array.Empty<ExperienceDto>(),
            p.Certifications?.Select(c => new CertificationDto(c.Id, c.Name, c.IssuingOrganization, c.IssueDate, c.CredentialUrl)) ?? Array.Empty<CertificationDto>(),
            p.UserSkills?.Select(s => new SkillDto(s.SkillId, s.Skill?.Name ?? string.Empty)) ?? Array.Empty<SkillDto>()
        );
    }
    public async Task AddSkillAsync(Guid userId, Guid skillId, CancellationToken cancellationToken = default)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        await _unitOfWork.UserProfiles.AddSkillToProfileAsync(profile.Id, skillId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.UserProfiles.EndorseSkillAsync(targetProfileId, skillId, endorsedByUserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveCvUrlToDbAsync(Guid userID, string url, CancellationToken cancellationToken = default)
    {
        var profile = await GetProfileByUserIdAsync(userID);
        var profileId = profile.Id;

        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("url can not be empty");
        }
        await _unitOfWork.UserProfiles.SaveCvUrlToDbAsync(profileId, url, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<string?> UpdateCvUrlInDbAsync(Guid userId, string newUrl, CancellationToken token)
    {
        var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, token);
        if (profile == null) throw new KeyNotFoundException("Profile not found.");

        var oldUrl = profile.CvFileUrl;

        profile.CvFileUrl = newUrl;

        await _unitOfWork.SaveChangesAsync(token);

        return oldUrl;
    }

    public async Task<IEnumerable<UserProfileDto>> 
    SearchProfiles(
        string? searchItem, 
        string targetProgram, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken
        )
    {
        var profiles = await _unitOfWork.UserProfiles.SearchProfilesAsync(searchItem, targetProgram,pageNumber,pageSize, cancellationToken);
        if (profiles == null) throw new KeyNotFoundException("Could not load profiles matching criteria");

        return profiles.Select(x => new UserProfileDto(
            x.Id,
            x.UserId.ToString(),
            x.FirstName,
            x.LastName,
            x.SystemHeadline,
            x.AboutBio,
            x.Programme,
            x.StudentNumber
        ));
    }
}