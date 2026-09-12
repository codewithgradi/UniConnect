using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/connections")]
[Authorize]
public class ConnectionsController : ApiControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionsController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyConnections(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var connections = await _connectionService.GetUserConnectionsAsync(userId, cancellationToken);
        return Ok(connections);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var requests = await _connectionService.GetPendingRequestsAsync(userId, cancellationToken);
        return Ok(requests);
    }

    [HttpPost("{targetProfileId:guid}")]
    public async Task<IActionResult> SendRequest(Guid targetProfileId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"logged in  user : {CurrentUserId}");
            Console.WriteLine($"target user profile id  : {targetProfileId}");
            await _connectionService.SendConnectionRequestAsync(CurrentUserId, targetProfileId, cancellationToken);
            return Ok(new { message = "Connection request sent successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPatch("accept/{requesterId:guid}")]
    public async Task<IActionResult> AcceptRequest(Guid requesterId, CancellationToken cancellationToken)
    {
        try
        {
            await _connectionService.AcceptConnectionAsync(requesterId, CurrentUserId, cancellationToken);
            return Ok(new { message = "Connection request accepted." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("reject/{requesterId:guid}")]
    public async Task<IActionResult> RejectRequest(Guid requesterId, CancellationToken cancellationToken)
    {
        try
        {
            await _connectionService.RejectConnectionAsync(requesterId, CurrentUserId, cancellationToken);
            return Ok(new { message = "Connection request rejected." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{targetUserId:guid}")]
    public async Task<IActionResult> RemoveConnection(Guid targetUserId, CancellationToken cancellationToken)
    {
        try
        {
            await _connectionService.RemoveConnectionAsync(CurrentUserId, targetUserId, cancellationToken);
            return Ok(new { message = "Connection removed successfully." });
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