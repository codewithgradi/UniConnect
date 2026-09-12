namespace UniConnect.Application.Interfaces;


public interface IEmailService
{
    Task<bool> SendEmail(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken = default
    );
}