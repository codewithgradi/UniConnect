// Application/Services/ICertificateVerificationService.cs
using UniConnect.Application.DTOs;

public interface ICertificateVerificationService
{
    Task<CertificateVerificationResponse> VerifyAsync(Stream pdfStream);
}