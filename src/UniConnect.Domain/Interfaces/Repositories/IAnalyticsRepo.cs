
namespace UniConnect.Domain.Interfaces.Repositories;

public interface IAnalyticsRepository
{
    Task<AdminAnalyticsDto> GetAdminAnalyticsAsync(CancellationToken cancellationToken = default);
}