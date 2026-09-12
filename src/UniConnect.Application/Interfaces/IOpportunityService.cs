using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;

namespace UniConnect.Application.Services;

public interface IOpportunityService
{
    Task<GetOpportunityWithApplications> GetOppporttunityWithApplication(Guid opportunityId ,CancellationToken token);
    Task CreateOpportunityAsync(Guid businessProfileId, CreateOpportunityDto dto, CancellationToken cancellationToken = default);
    Task ApplyForJobAsync(Guid opportunityId, Guid applicantId, string cvFileUrl, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(string? targetProgramme, CancellationToken cancellationToken = default);
    Task<Opportunity?> GetByIdAsync(Guid opportunityId, CancellationToken cancellationToken = default);
    Task ApproveOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default);
    Task RejectOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default);
    Task CloseOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetPendingOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetMyOpportunitiesAsync(Guid businessProfileId, CancellationToken cancellationToken = default);
    }