using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IUserAnalyticsService
{
    Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BusinessAnalyticsDto> GetBusinessAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default);
}