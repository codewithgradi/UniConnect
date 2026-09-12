// Application/Services/CertificateVerificationService.cs
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;
using UniConnect.Application.DTOs;

public class CertificateVerificationService : ICertificateVerificationService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<CertificateVerificationService> _logger;
    private readonly string _sampleTemplatePath;

    public CertificateVerificationService(
        IChatClient chatClient,
        ILogger<CertificateVerificationService> logger,
        IConfiguration configuration)
    {
        _chatClient = chatClient;
        _logger = logger;
        _sampleTemplatePath = configuration["CertificateSettings:SampleTemplatePath"] ?? "Resources/richfield-template.jpg";
    }

    public async Task<CertificateVerificationResponse> VerifyAsync(Stream pdfStream)
    {
        // 1. Validate PDF format and orientation using PdfPig
        using var pdf = PdfDocument.Open(pdfStream);
        if (pdf.NumberOfPages == 0)
        {
            return new CertificateVerificationResponse(0, "scanned", DateTime.UtcNow);
        }

        var page = pdf.GetPage(1);

        // Ensure page is not landscape (Width must not be greater than Height)
        if (page.Width > page.Height)
        {
            return new CertificateVerificationResponse(2, "scanned", DateTime.UtcNow);
        }

        // 2. Load the official sample template image bytes if available
        byte[]? sampleImageBytes = null;
        if (File.Exists(_sampleTemplatePath))
        {
            sampleImageBytes = await File.ReadAllBytesAsync(_sampleTemplatePath);
        }
        else
        {
            _logger.LogWarning("Reference sample template not found at {Path}", _sampleTemplatePath);
        }

        // 3. Extract text for validation fallback
        var extractedText = string.Join(" ", page.GetWords().Select(w => w.Text));

        // 4. Construct Multimodal Prompt
        var chatContents = new List<AIContent>
        {
            new TextContent("Please cross-check the uploaded certificate document against the reference sample template and evaluate strict criteria: Full Name, Qualification title, Date of issue, Place of issue (Durban), SAQA ID (35954), NQF Level (7), Richfield logo and title, student reference, and signatures of Chief Academic Officer (S Chengadu) and Group Academic Registrar.")
        };

        if (sampleImageBytes != null)
        {
            chatContents.Add(new TextContent("Official reference sample template:"));
            chatContents.Add(new DataContent(sampleImageBytes, "image/jpeg"));
        }

        chatContents.Add(new TextContent($"Extracted text from uploaded file: {extractedText}"));
        chatContents.Add(new TextContent("Provide an objective credibility score out of 10. Respond ONLY with a raw JSON object in this exact format: {\"score\": <int>}"));

        var chatMessages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an expert document credibility verification AI for Richfield Graduate Institute of Technology."),
            new ChatMessage(ChatRole.User, chatContents)
        };

        // Explicitly lower max_tokens to fit within OpenRouter credit limits (e.g., limit output generation tokens)
        var chatOptions = new ChatOptions
        {
            MaxOutputTokens = 150
        };

        var response = await _chatClient.GetResponseAsync(chatMessages, chatOptions);
        int score = 5;

        try
        {
            var cleanJson = response.Text?.Trim().Trim('`').Replace("json", "").Trim();
            var jsonDoc = JsonDocument.Parse(cleanJson ?? "{\"score\": 5}");
            if (jsonDoc.RootElement.TryGetProperty("score", out var scoreProp))
            {
                score = scoreProp.GetInt32();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI credibility score response.");
        }

        return new CertificateVerificationResponse(
            CredibilityScore: Math.Clamp(score, 0, 10),
            DocumentStatus: "scanned",
            Time: DateTime.UtcNow
        );
    }
}