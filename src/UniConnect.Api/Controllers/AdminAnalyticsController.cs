using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/admin/analytics")]
[Authorize(Roles = "Admin")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AdminAnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAnalytics(CancellationToken cancellationToken)
    {
        var stats = await _analyticsService.GetAdminAnalyticsAsync(cancellationToken);
        return Ok(stats);
    }
}