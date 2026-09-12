using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;
using UniConnect.Domain.Interfaces.Repositories;
namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
[Authorize]
public class OpportunitiesController : ControllerBase
{
    private readonly IOpportunityService _opportunityService;
    private readonly IUnitOfWork _iUnitOfWork;

    public OpportunitiesController(IOpportunityService opportunityService, IUnitOfWork UnitOfWork)
    {
        _opportunityService = opportunityService;
        _iUnitOfWork = UnitOfWork;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveOpportunities([FromQuery] string? targetProgramme, CancellationToken cancellationToken)
    {
        var opportunities = await _opportunityService.GetActiveOpportunitiesAsync(targetProgramme, cancellationToken);
        return Ok(opportunities);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityService.GetByIdAsync(id, cancellationToken);
        if (opportunity == null) return NotFound();

        return Ok(opportunity);
    }

    [HttpPost]
    [Authorize(Roles = "Business,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOpportunityDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var businessProfile = await _iUnitOfWork.BusinessProfiles.GetByUserIdAsync(userId, cancellationToken);

        if (businessProfile == null)
        {
            return BadRequest(new { message = "No associated business profile found for this user." });
        }

        await _opportunityService.CreateOpportunityAsync(businessProfile.Id, dto, cancellationToken);

        return Ok(new { message = "Opportunity submitted for approval." });
    }

    [HttpPost("{id:guid}/apply")]
    [Authorize(Roles = "Student,Alumni")]
    public async Task<IActionResult> Apply(Guid id, CancellationToken cancellationToken)
    {
        var applicantId = GetCurrentUserId();
        var user = await _iUnitOfWork.UserProfiles.GetByUserIdAsync(applicantId, cancellationToken);
        var cvFileUrl = user?.CvFileUrl;


        try
        {
            await _opportunityService.ApplyForJobAsync(id, applicantId, cvFileUrl, cancellationToken);
            return Ok(new { message = "Application submitted successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingOpportunities(CancellationToken cancellationToken)
    {
        var pending = await _opportunityService.GetPendingOpportunitiesAsync(cancellationToken);
        return Ok(pending);
    }
    [HttpGet("{opoId:guid}/applications")]
    public async Task<IActionResult> GetWithApplications( [FromRoute] Guid opoId, CancellationToken cancellationToken)
    {
      var res = await _opportunityService.GetOppporttunityWithApplication(opoId,cancellationToken);
      if(res == null) return NotFound("oportunity was not found");
      return Ok(res);
    }

    [HttpGet("my-postings")]
    [Authorize(Roles = "Business")]
    public async Task<IActionResult> GetMyPostings(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var businessProfile = await _iUnitOfWork.BusinessProfiles.GetByUserIdAsync(userId, cancellationToken);

        if (businessProfile == null)
        {
            return BadRequest(new { message = "No associated business profile found." });
        }

        var myOpportunities = await _opportunityService.GetMyOpportunitiesAsync(businessProfile.Id, cancellationToken);
        return Ok(myOpportunities);
    }
    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _opportunityService.ApproveOpportunityAsync(id, cancellationToken);
            return Ok(new { message = "Opportunity published successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _opportunityService.RejectOpportunityAsync(id, cancellationToken);
            return Ok(new { message = "Opportunity rejected." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/close")]
    [Authorize(Roles = "Business,Admin")]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _opportunityService.CloseOpportunityAsync(id, cancellationToken);
            return Ok(new { message = "Opportunity closed." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(nameIdentifier, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Invalid or missing user ID claim.");
    }
}