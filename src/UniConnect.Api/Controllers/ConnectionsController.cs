using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Connections")]
public class ConnectionsController : ApiControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionsController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    [HttpPost("request/{receiverId:guid}")]
    public async Task<IActionResult> SendRequest(Guid receiverId)
    {
        await _connectionService.SendConnectionRequestAsync(CurrentUserId, receiverId);
        return Ok(new { Message = "Connection request sent." });
    }

    [HttpPost("accept/{requesterId:guid}")]
    public async Task<IActionResult> AcceptRequest(Guid requesterId)
    {
        await _connectionService.AcceptConnectionAsync(requesterId, CurrentUserId);
        return Ok(new { Message = "Connection accepted." });
    }
}