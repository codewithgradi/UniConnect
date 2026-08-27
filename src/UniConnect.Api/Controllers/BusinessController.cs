using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Business")]
public class BusinessController : ApiControllerBase
{
    private readonly IBusinessService _businessService;

    public BusinessController(IBusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyBusinessProfile()
    {
        var business = await _businessService.GetByUserIdAsync(CurrentUserId);
        if (business == null) return NotFound("Business profile not found.");
        return Ok(business);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusinessProfile([FromBody] CreateBusinessDto dto)
    {
        await _businessService.CreateBusinessProfileAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetMyBusinessProfile), null, new { Message = "Business profile created." });
    }
}