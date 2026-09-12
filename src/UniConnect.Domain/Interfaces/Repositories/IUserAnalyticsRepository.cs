
namespace UniConnect.Domain.Interfaces.Repositories;

public interface IUserAnalyticsRepository
{
    Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BusinessAnalyticsDto> GetBusinessAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default);
}