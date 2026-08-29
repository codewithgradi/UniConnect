using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UniConnect.Api.Hubs;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Institutional Events")]
public class InstitutionalController : ApiControllerBase
{
    private readonly IInstitutionalService _institutionalService;
    private readonly IHubContext<ChatHub> _hubContext; 

    public InstitutionalController(IInstitutionalService institutionalService, IHubContext<ChatHub> hubContext)
    {
        _institutionalService = institutionalService;
        _hubContext=hubContext;
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
        await _hubContext.Clients.All.SendAsync("ReceiveSystemEventAnnouncement", new
        {
            Title = dto.Title,
            Content = dto.Description,
            SentBy = "System Administrator",
            SentAtUtc = DateTime.UtcNow
        });
        return Ok(new { Message = "Campus event created." });
    }
}