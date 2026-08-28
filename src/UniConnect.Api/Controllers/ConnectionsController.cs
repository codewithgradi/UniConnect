using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/connections")]
[Authorize]
public class ConnectionsController : ControllerBase
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

    [HttpPost("request/{receiverId:guid}")]
    public async Task<IActionResult> SendRequest(Guid receiverId, CancellationToken cancellationToken)
    {
        var requesterId = GetCurrentUserId();
        try
        {
            await _connectionService.SendConnectionRequestAsync(requesterId, receiverId, cancellationToken);
            return Ok(new { message = "Connection request sent." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("accept/{requesterId:guid}")]
    public async Task<IActionResult> AcceptRequest(Guid requesterId, CancellationToken cancellationToken)
    {
        var receiverId = GetCurrentUserId();
        try
        {
            await _connectionService.AcceptConnectionAsync(requesterId, receiverId, cancellationToken);
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
        var receiverId = GetCurrentUserId();
        try
        {
            await _connectionService.RejectConnectionAsync(requesterId, receiverId, cancellationToken);
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
        var userId = GetCurrentUserId();
        try
        {
            await _connectionService.RemoveConnectionAsync(userId, targetUserId, cancellationToken);
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