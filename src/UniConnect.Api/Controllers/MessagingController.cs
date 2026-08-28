using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessagingService _messagingService;

    public MessagesController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
    {
        var senderId = GetCurrentUserId();
        try
        {
            var result = await _messagingService.SendMessageAsync(senderId, dto.ReceiverId, dto.Content, cancellationToken);
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