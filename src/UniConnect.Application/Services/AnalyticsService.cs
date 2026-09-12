using UniConnect.Application.DTOs;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public AnalyticsService(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<AdminAnalyticsDto> GetAdminAnalyticsAsync(CancellationToken cancellationToken = default)
    {
        return await _analyticsRepository.GetAdminAnalyticsAsync(cancellationToken);
    }
}