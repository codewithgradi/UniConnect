using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;

[ApiController]
[Route("api/certificates")]
[Tags("Certificates")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateVerificationService _verificationService;

    public CertificatesController(ICertificateVerificationService verificationService)
    {
        _verificationService = verificationService;
    }
    [HttpPost("verify")]
    [ProducesResponseType(typeof(CertificateVerificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyCertificate([FromBody] UploadCertificateJsonDto request)
    {
        if (string.IsNullOrEmpty(request.Base64File))
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        if (!Path.GetExtension(request.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Only PDF files are supported." });
        }

        var bytes = Convert.FromBase64String(request.Base64File);
        using var stream = new MemoryStream(bytes);
        var result = await _verificationService.VerifyAsync(stream);

        return Ok(result);
    }

   
}
public class UploadCertificateJsonDto
{
    public required string Base64File { get; set; }
    public required string FileName { get; set; }
}