using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Profiles")]
public class ProfileController : ApiControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _profileService.GetProfileByUserIdAsync(CurrentUserId);
        if (profile == null) return NotFound("Profile not found.");
        return Ok(profile);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetProfileByUserId(Guid userId)
    {
        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        if (profile == null) return NotFound("Profile not found.");
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        await _profileService.UpdateProfileAsync(CurrentUserId, dto);
        return NoContent();
    }

    [HttpPost("me/skills/{skillId:guid}")]
    public async Task<IActionResult> AddSkill(Guid skillId)
    {
        await _profileService.AddSkillAsync(CurrentUserId, skillId);
        return Ok(new { Message = "Skill added successfully." });
    }

    [HttpPost("profiles/{profileId:guid}/skills/{skillId:guid}/endorse")]
    public async Task<IActionResult> EndorseSkill(Guid profileId, Guid skillId)
    {
        await _profileService.EndorseSkillAsync(profileId, skillId, CurrentUserId);
        return Ok(new { Message = "Skill endorsed successfully." });
    }
}