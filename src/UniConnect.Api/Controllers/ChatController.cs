using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using UniConnect.Api.Controllers;
using UniConnect.Api.Mcp;

namespace GradiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ApiControllerBase
{
    private readonly IChatClient _chatClient;
    private readonly UserProfileMcpTool _mcpTool;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IChatClient chatClient,
        UserProfileMcpTool mcpTool,
        ILogger<ChatController> logger)
    {
        _chatClient = chatClient;
        _mcpTool = mcpTool;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        // 1. Basic Request Validation
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ChatResponseDto(
                Success: false,
                Reply: null,
                Error: "Message prompt cannot be empty."
            ));
        }

        _logger.LogInformation("Processing chat request with prompt: {Prompt}", request.Message);

        // 2. Define System Role & Conversation Context
        List<ChatMessage> conversation = new()
        {
            new ChatMessage(ChatRole.System,
            """
            You are an empathetic, knowledgeable, 
            and conversational AI Career & Student Advisor for the 
            UniConnect platform. 
            Your role is to help students and alumni 
            navigate their academic journey, 
            career planning, job applications,
            and personal branding.

            ### CORE OPERATIONAL RULES:

            1. CONVERSATIONAL & HUMANE TONE:
            - Speak naturally, like a mentor or peer. 
            Avoid overly formal corporate speak, robotic list dumps,
            or dry database summaries.
            - Tailor your guidance directly to the user's current situation
                using the profile details provided via tools.

            2. NEVER EXPOSE RAW IDENTIFIERS OR TECHNICAL DATA:
            - NEVER output database IDs, GUIDs, sequence numbers, or system keys in your responses.
            - Refer to entities by their natural, human names.
            -Never return sepcial character in you response such as ge "/n"

            3. PROVIDING ACTIONABLE ADVICE & "HOW TO STAND OUT":
            - When asked for advice, analyze the user's full profile (education, experience, certifications, and skills).
            - Keep suggestions practical, focused, and organized into small, readable chunks.

            4. TOOL USAGE:
            - Always call the available `GetBasicInfo` tool to fetch full profile details for the logged-in user before formulating profile-related answers.
            - Process the JSON response internally and summarize in natural language.
            """),
            new ChatMessage(ChatRole.User, request.Message)
        };

        // 3. Register Available Tools / AI Functions with Token Cap
        var chatOptions = new ChatOptions
        {
            MaxOutputTokens = 1000, // Essential for OpenRouter tool execution
            Tools = new List<AITool>
            {
                AIFunctionFactory.Create(
                    async () => await _mcpTool.GetFullUserProfile(CurrentUserId),
                    name: "GetBasicInfo",
                    description: "Fetches basic profile information, background, and personal context for the logged-in user."
                )
            }
        };

        // 4. Send Request to Model
        ChatResponse response = await _chatClient.GetResponseAsync(
            conversation,
            chatOptions,
            cancellationToken
        );

        _logger.LogInformation("Chat completion successful.");

        return Ok(new ChatResponseDto(
            Success: true,
            Reply: response.Text,
            Error: null
        ));
    }
}

public record ChatRequest(string Message);

public record ChatResponseDto(
    bool Success,
    string? Reply,
    string? Error
);