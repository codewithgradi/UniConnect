using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Institutional Events")]
public class InstitutionalController : ApiControllerBase
{
    private readonly IInstitutionalService _institutionalService;

    public InstitutionalController(IInstitutionalService institutionalService)
    {
        _institutionalService = institutionalService;
    }

    [HttpGet("events")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var events = await _institutionalService.GetUpcomingEventsAsync();
        return Ok(events);
    }

    [HttpPost("events")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        await _institutionalService.CreateEventAsync(CurrentUserId, dto);
        return Ok(new { Message = "Campus event created." });
    }
}