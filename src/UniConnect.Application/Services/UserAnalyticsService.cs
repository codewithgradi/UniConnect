using UniConnect.Application.DTOs;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class UserAnalyticsService : IUserAnalyticsService
{
    private readonly IUserAnalyticsRepository _repository;

    public UserAnalyticsService(IUserAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetStudentAnalyticsAsync(userId, cancellationToken);
    }

    public async Task<BusinessAnalyticsDto> GetBusinessAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetBusinessAnalyticsAsync(userId, cancellationToken);
    }
}