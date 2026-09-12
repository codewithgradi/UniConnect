using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSkills(CancellationToken cancellationToken)
    {
        var skills = await _skillService.GetAllAsync(cancellationToken);
        return Ok(skills);
    }

    // Admin-only creation endpoint
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var createdSkill = await _skillService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAllSkills), new { id = createdSkill.Id }, createdSkill);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin-only deletion endpoint
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSkill(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _skillService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}