using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IAnalyticsService
{
    Task<AdminAnalyticsDto> GetAdminAnalyticsAsync(CancellationToken cancellationToken = default);
}