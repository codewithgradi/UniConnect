using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IOpportunityService
{
    Task CreateOpportunityAsync(Guid businessProfileId, CreateOpportunityDto dto);
    Task ApplyForJobAsync(Guid opportunityId, Guid applicantId, string cvFileUrl);
}