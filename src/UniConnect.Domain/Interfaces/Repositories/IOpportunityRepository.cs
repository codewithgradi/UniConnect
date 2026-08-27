using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IOpportunityRepository
{
    Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Opportunity?> GetWithApplicationsAsync(Guid opportunityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(string? targetProgramme, CancellationToken cancellationToken = default);
    Task AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default);

    // Application Management
    Task AddApplicationAsync(JobApplication application, CancellationToken cancellationToken = default);
    Task<bool> HasUserAppliedAsync(Guid opportunityId, Guid userId, CancellationToken cancellationToken = default);
}