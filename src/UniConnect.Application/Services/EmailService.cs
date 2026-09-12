using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using UniConnect.Application.Interfaces;
namespace UniConnect.Application.Services;

public class EmailService : IEmailService
{
    private readonly BrevoSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<BrevoSettings> settings,
        HttpClient httpClient,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException("BrevoSettings:ApiKey is missing.");
        }
    }

    public async Task<bool> SendEmail(string toEmail, string subject, string htmlContent, CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                sender = new { name = _settings.SenderName, email = _settings.SenderEmail },
                to = new[] { new { email = toEmail.Trim() } },
                subject = subject,
                htmlContent = htmlContent
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl);
            request.Headers.Add("api-key", _settings.ApiKey.Trim());
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Brevo API failure [{StatusCode}]: {Error}", response.StatusCode, errorResponse);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch email via Brevo HTTP API.");
            return false;
        }
    }
}
public class BrevoSettings
{
    public const string SectionName = "BrevoSettings";

    public string ApiKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "UniConnect";
    public string ApiUrl { get; set; } = "https://api.brevo.com/v3/smtp/email";
}