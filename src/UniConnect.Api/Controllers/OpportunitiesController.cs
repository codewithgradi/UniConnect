using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Opportunities")]
public class OpportunitiesController : ApiControllerBase
{
    private readonly IOpportunityService _opportunityService;

    public OpportunitiesController(IOpportunityService opportunityService)
    {
        _opportunityService = opportunityService;
    }

    [HttpPost("business/{businessProfileId:guid}")]
    [Authorize(Roles = "Business,Admin")]
    public async Task<IActionResult> CreateOpportunity(Guid businessProfileId, [FromBody] CreateOpportunityDto dto)
    {
        await _opportunityService.CreateOpportunityAsync(businessProfileId, dto);
        return Ok(new { Message = "Job opportunity posted." });
    }

    [HttpPost("{opportunityId:guid}/apply")]
    public async Task<IActionResult> Apply(Guid opportunityId, [FromBody] ApplyJobRequest request)
    {
        await _opportunityService.ApplyForJobAsync(opportunityId, CurrentUserId, request.CvFileUrl);
        return Ok(new { Message = "Job application submitted." });
    }
}

public record ApplyJobRequest(string CvFileUrl);