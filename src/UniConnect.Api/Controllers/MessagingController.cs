using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Messaging")]
public class MessagingController : ApiControllerBase
{
    private readonly IMessagingService _messagingService;

    public MessagingController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpGet("conversation/{recipientId:guid}")]
    public async Task<IActionResult> GetConversation(Guid recipientId)
    {
        var conversation = await _messagingService.GetConversationAsync(CurrentUserId, recipientId);
        return Ok(conversation);
    }

    [HttpPost("send/{recipientId:guid}")]
    public async Task<IActionResult> SendMessage(Guid recipientId, [FromBody] SendMessageRequest request)
    {
        await _messagingService.SendMessageAsync(CurrentUserId, recipientId, request.Content);
        return Ok(new { Message = "Message sent successfully." });
    }
}

public record SendMessageRequest(string Content);