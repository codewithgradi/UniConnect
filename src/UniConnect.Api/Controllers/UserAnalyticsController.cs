using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class UserAnalyticsController : ControllerBase
{
    private readonly IUserAnalyticsService _analyticsService;

    public UserAnalyticsController(IUserAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student,Alumni")]
    public async Task<IActionResult> GetStudentAnalytics(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var stats = await _analyticsService.GetStudentAnalyticsAsync(userId, cancellationToken);
        return Ok(stats);
    }

    [HttpGet("business")]
    [Authorize(Roles = "Business")]
    public async Task<IActionResult> GetBusinessAnalytics(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        try
        {
            var stats = await _analyticsService.GetBusinessAnalyticsAsync(userId, cancellationToken);
            return Ok(stats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(claim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User identifier missing or invalid.");
    }
}