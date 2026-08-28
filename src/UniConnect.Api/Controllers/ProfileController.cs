using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/profiles")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.CreateProfileAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetMyProfile), null, profile);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileByUserIdAsync(userId, cancellationToken);
        if (profile == null) return NotFound(new { message = "Profile not found." });

        return Ok(profile);
    }

    [HttpGet("{profileId:guid}")]
    public async Task<IActionResult> GetProfileById(Guid profileId, CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileByIdAsync(profileId, cancellationToken);
        if (profile == null) return NotFound(new { message = "Profile not found." });

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _profileService.UpdateProfileAsync(userId, dto, cancellationToken);
        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPost("me/experiences")]
    public async Task<IActionResult> AddExperience([FromBody] AddExperienceDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var exp = await _profileService.AddExperienceAsync(userId, dto, cancellationToken);
        return Ok(exp);
    }

    [HttpDelete("me/experiences/{experienceId:guid}")]
    public async Task<IActionResult> DeleteExperience(Guid experienceId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _profileService.DeleteExperienceAsync(userId, experienceId, cancellationToken);
        return NoContent();
    }

    [HttpPost("me/certifications")]
    public async Task<IActionResult> AddCertification([FromBody] AddCertificationDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var cert = await _profileService.AddCertificationAsync(userId, dto, cancellationToken);
        return Ok(cert);
    }

    [HttpDelete("me/certifications/{certificationId:guid}")]
    public async Task<IActionResult> DeleteCertification(Guid certificationId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _profileService.DeleteCertificationAsync(userId, certificationId, cancellationToken);
        return NoContent();
    }

    [HttpPost("me/skills/{skillId:guid}")]
    public async Task<IActionResult> AddSkill(Guid skillId)
    {
        var userId = GetCurrentUserId();
        await _profileService.AddSkillAsync(userId, skillId);
        return Ok(new { message = "Skill added to profile." });
    }

    [HttpPost("{targetProfileId:guid}/skills/{skillId:guid}/endorse")]
    public async Task<IActionResult> EndorseSkill(Guid targetProfileId, Guid skillId)
    {
        var endorserUserId = GetCurrentUserId();
        await _profileService.EndorseSkillAsync(targetProfileId, skillId, endorserUserId);
        return Ok(new { message = "Skill endorsed successfully." });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(claim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User identifier claim missing or invalid.");
    }
}