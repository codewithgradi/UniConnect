using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UniConnect.Api.Hubs;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessagingService _messagingService;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessagesController(IMessagingService messagingService, IHubContext<ChatHub> hubContext)
    {
        _messagingService = messagingService;
        _hubContext = hubContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        var res = await _messagingService.GetAllMessages(currentUserId, cancellationToken);
        if (res != null) return Ok(res);
        else return BadRequest("Could not get messages");
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
    {
        var senderId = GetCurrentUserId();
        try
        {
            var result = await _messagingService.SendMessageAsync(senderId, dto.ReceiverId, dto.Content, cancellationToken);
            await _hubContext.Clients.User(dto.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                id = result.Id,
                senderId = result.SenderId,
                content = result.Content,
                sentAt = result.SentAt
            });
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("conversation/{otherUserId:guid}")]
    public async Task<IActionResult> GetConversation(
        Guid otherUserId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var messages = await _messagingService.GetConversationAsync(currentUserId, otherUserId, skip, take, cancellationToken);
        return Ok(messages);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        var count = await _messagingService.GetUnreadCountAsync(currentUserId, cancellationToken);
        return Ok(new { unreadCount = count });
    }

    [HttpPatch("read")]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadDto dto, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        await _messagingService.MarkMessagesAsReadAsync(currentUserId, dto.MessageIds, cancellationToken);
        return Ok(new { message = "Messages marked as read." });
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