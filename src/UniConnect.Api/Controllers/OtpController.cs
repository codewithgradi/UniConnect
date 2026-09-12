using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Interfaces;
using UniConnect.Application.Services;
[ApiController]
[Route("api/[controller]")]
[Tags("Otp")]
public class OtpController : ControllerBase
{
    private IOtpService _otpService;
    private readonly IEmailService _emailService;

    public OtpController(IOtpService otpService, IEmailService emailService)
    {
        _otpService = otpService;
        _emailService = emailService;
    }
    // 1. Endpoint to Request OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request,
        [FromServices] IOtpService otpService,
        [FromServices] IEmailService emailService,
        CancellationToken ct)
    {

        var email = request.Email?.Trim();

        // Regex check for student email format
        if (string.IsNullOrWhiteSpace(email) ||
            !Regex.IsMatch(email, @"^\d{7,10}@my\.richfield\.ac\.za$", RegexOptions.IgnoreCase))
        {
            return BadRequest(new { Message = "Must be a valid student email (@my.richfield.ac.za)" });
        }

        // Generate random 6-digit code
        var otpCode = Random.Shared.Next(100000, 999999).ToString();

        // Save code to Redis with 10-minute TTL
        await otpService.SaveOtpAsync(email, otpCode, ct);

        // Send email
        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2>UniConnect Student Verification</h2>
            <p>Your verification code is:</p>
            <h1 style='color: #0d233a; font-size: 32px; letter-spacing: 5px;'>{otpCode}</h1>
            <p>This code will expire in 10 minutes.</p>
        </div>";

        var sent = await emailService.SendEmail(email, "Your Verification Code", htmlBody, ct);

        return sent
            ? Ok(new { Message = "OTP sent to student email." })
            : BadRequest("Failed to deliver email.");

    }
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
    [FromBody] VerifyOtpRequest request,
    [FromServices] IOtpService otpService,
    CancellationToken ct
    )
    {
        var isValid = await otpService.ValidateOtpAsync(request.Email, request.Otp, ct);

        if (!isValid)
        {
            return BadRequest(new { Message = "Invalid or expired verification code." });
        }

        // OTP was valid and is now deleted from Redis!
        // TODO: Mark user account as email-verified or generate JWT token
        return Ok(new { Message = "Student email verified successfully." });
    }

}



public record SendOtpRequest(string Email);
public record VerifyOtpRequest(string Email, string Otp);